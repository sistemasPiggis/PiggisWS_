namespace PIGGISWS.Services.Utils;

using Google.Apis.Auth.OAuth2;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using PIGGISWS.Data;
using Microsoft.EntityFrameworkCore;
using PIGGISWS.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PIGGISWS.Interfaces;
using PIGGISWS.Models.DTOs;

public class FirebaseNotificationService : IFirebaseNotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly ILogger<FirebaseNotificationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IAgenteService _agenteService;

    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;
    int p_not_inactivo;
    int p_not_procesada;

    // Credenciales de tu App Principal (Actual)
    private string _serviceAccountKeyPath;
    private string _projectId;

    private string _AppAgentesJsonPath;
    private string _AppAgentesProjectId;
    public FirebaseNotificationService(IConfiguration configuration, HttpClient httpClient, ApplicationDbContext context, 
                                                        ILogger<FirebaseNotificationService> logger, IAgenteService agenteService)
    {
        _httpClient = httpClient;
        _context = context;
        _logger = logger;
        _configuration = configuration;

        CargarConfiguracion();
        GetParametros();
        _agenteService = agenteService;
    }

    private void CargarConfiguracion()
    {

        _serviceAccountKeyPath = _configuration["Firebase:ServiceAccountKeyPath"];
        _projectId = _configuration["Firebase:ProjectId"] ?? "977990401928";

        _AppAgentesJsonPath = _configuration["Firebase:AppAgentesJsonPath"];
        _AppAgentesProjectId = _configuration["Firebase:AppAgentesProjectId"];
    }

    public void GetParametros()
    {
        parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "FirebaseNotificationService" || p.SERVICIO == "GENERAL").ToList();
        p_not_inactivo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 20)?.VALOR ?? "100");
        p_not_procesada = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 21)?.VALOR ?? "100");
        p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
    }

    public async Task<string> GetAccessTokenAsync()
    {
        return await GetAccessTokenPersonalizadoAsync(_serviceAccountKeyPath);
    }


    private async Task<string> GetAccessTokenPersonalizadoAsync(string pathJson)
    {
        using var stream = new FileStream(pathJson, FileMode.Open, FileAccess.Read);
        var credential = GoogleCredential.FromStream(stream).CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
        return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
    }

    private async Task<bool> EnviarMensajeCoreAsync(string fcmToken, string titulo, string cuerpo, string projectId, string tokenAccesoGoogle)
    {
        try
        {
            string url = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";

            var message = new
            {
                message = new
                {
                    token = fcmToken,
                    notification = new
                    {
                        title = titulo,
                        body = cuerpo
                    }
                }
            };

            var jsonMessage = JsonSerializer.Serialize(message);


            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenAccesoGoogle);
            request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al enviar a {fcmToken}: {errorResponse}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico enviando mensaje a Firebase.");
            return false;
        }
    }


    public async Task<ServiceResponse<object>> SendFcmMessageAsync(long not_codigo)
    {
        var _response = new ServiceResponse<object>();
        int contador = 0;
        try
        {
            var listado = await (from n in _context.NOTIFICACIONES
                                 join ng in _context.NOTIFICACIONES_GRUPOS on n.NOT_CODIGO equals ng.NOT_NOT_CODIGO
                                 join em in _context.EMPLEADO on ng.NOT_EMP_CODIGO equals em.EMP_CODIGO
                                 join fc in _context.FCM_TOKEN on em.EMP_CODIGO.ToString() equals fc.FCM_EMP_CODIGO
                                 where n.NOT_CODIGO == not_codigo
                                 && n.NOT_INACTIVO == p_not_inactivo
                                 && n.NOT_PROCESADA == p_not_procesada
                                 && (n.APP_DESTINO == null || n.APP_DESTINO != 1)
                                 && (fc.APP == null || fc.APP == "")
                                 select new
                                 {
                                     n.NOT_COMUNICADO,
                                     n.NOT_OBSERVACIONES,
                                     n.NOT_DESCRIPCION,
                                     fc.FCM_TOKEN
                                 }).ToListAsync();

            if (listado == null || !listado.Any())
            {
                _response.Message = "No existe notificación en la base de datos o esta inactiva";
                _response.Success = true;
                return _response;
            }

            // Generamos un solo token de Google para todo el bloque (Optimización de velocidad)
            var accessToken = await GetAccessTokenPersonalizadoAsync(_serviceAccountKeyPath);

            foreach (var item in listado)
            {
                // Llamamos al motor pasándole los datos por defecto
                bool exito = await EnviarMensajeCoreAsync(item.FCM_TOKEN, item.NOT_COMUNICADO, item.NOT_DESCRIPCION, _projectId, accessToken);
                if (exito)
                {
                    contador++;
                    Console.WriteLine("Mensaje enviado correctamente. " + contador);
                }
            }

            var notificacion = _context.NOTIFICACIONES.Find(p_empresa, not_codigo);
            if (notificacion != null)
            {
                notificacion.NOT_INACTIVO = 1;
                _context.SaveChanges();
            }

            _response.Message = "Notificación enviada correctamente el número de notificaciones es:" + contador;
            _response.Success = true;
        }
        catch (Exception ex)
        {
            _response.Message = ex.ToString();
            _response.Success = false;
        }
        return _response;
    }

    public async Task<ServiceResponse<object>> SendALLFcmMessageAsync()
    {
        var _response = new ServiceResponse<object>();
        DateTime FECHA = DateTime.Now.AddDays(-5);
        string nocontiene = "PLANIFICACIONES";
        int contador = 0;

        try
        {
            var listado = await (from n in _context.NOTIFICACIONES
                                 join ng in _context.NOTIFICACIONES_GRUPOS on n.NOT_CODIGO equals ng.NOT_NOT_CODIGO
                                 join em in _context.EMPLEADO on ng.NOT_EMP_CODIGO equals em.EMP_CODIGO
                                 join fc in _context.FCM_TOKEN on em.EMP_CODIGO.ToString() equals fc.FCM_EMP_CODIGO
                                 where !n.NOT_DESCRIPCION.ToUpper().Contains(nocontiene)
                                 && n.NOT_INACTIVO == p_not_inactivo
                                 && n.NOT_PROCESADA == p_not_procesada
                                 && n.NOT_COMUNICADO != null
                                 && n.CREA_FECHA >= FECHA
                                 && (n.APP_DESTINO == null || n.APP_DESTINO != 1)
                                 && (fc.APP == null || fc.APP == "")
                                 select new
                                 {
                                     n.NOT_COMUNICADO,
                                     n.NOT_DESCRIPCION,
                                     fc.FCM_TOKEN,
                                     n.NOT_CODIGO
                                 }).ToListAsync();

            if (listado == null || !listado.Any())
            {
                _response.Message = "No existe notificación en la base de datos o esta inactiva";
                _response.Success = true;
                _logger.LogInformation("Se ha ejecutado SendALLFcmMessageAsync. " + _response.Message);
                return _response;
            }

            var accessToken = await GetAccessTokenPersonalizadoAsync(_serviceAccountKeyPath);

            
                foreach (var item in listado)
                {
                    bool exito = await EnviarMensajeCoreAsync(item.FCM_TOKEN, item.NOT_COMUNICADO, item.NOT_DESCRIPCION, _projectId, accessToken);
                    if (exito)
                    {
                        contador++;
                        Console.WriteLine("Mensaje enviado correctamente. " + contador);
                    }
                }


            var codigosUnicos = listado.Select(x => x.NOT_CODIGO).Distinct().ToList();

            if (codigosUnicos.Any())
            {

                var notificacionesAActualizar = await _context.NOTIFICACIONES
                    .Where(n => n.NOT_EMPRESA == p_empresa && codigosUnicos.Contains(n.NOT_CODIGO))
                    .ToListAsync();

                foreach (var notificacion in notificacionesAActualizar)
                {
                    notificacion.NOT_PROCESADA = 1;
                }

                await _context.SaveChangesAsync();
            }


            _response.Message = "Notificación enviada correctamente el número de notificaciones es:" + contador;
            _response.Success = true;
            _logger.LogInformation("Se ha ejecutado SendALLFcmMessageAsync.");
        }
        catch (Exception ex)
        {
            _response.Message = ex.ToString();
            _response.Success = false;
            _logger.LogError(ex, "Error al ejecutar SendALLFcmMessageAsync.");
        }
        return _response;
    }

    public async Task<ServiceResponse<object>> RegistrarAPPAsync(Fcm_Token requestToken)
    {
        var _response = new ServiceResponse<object>();
        try
        {
            decimal empleadoId = 0;
            string usuario = "";
            bool esLogout = requestToken.FCM_EMP_CODIGO == "0" || string.IsNullOrEmpty(requestToken.FCM_EMP_CODIGO);
            if (!esLogout)
            {
                if (decimal.TryParse(requestToken.FCM_EMP_CODIGO, out decimal agenteCodigo))
                {
                    var agentes = await (from e in _context.AGENTE
                                         where e.AGE_CODIGO == agenteCodigo && e.AGE_INACTIVO == 0
                                         select e.AGE_EMPLEADO).ToListAsync();

                    empleadoId = agentes.FirstOrDefault() ?? 0;
                    usuario = await _agenteService.GetUsuarioAsync(agenteCodigo);
                }

                if (empleadoId == 0)
                {
                    _response.Message = "Error: No se encontró un empleado activo para este agente.";
                    _response.Success = false;
                    return _response;
                }
            }
            var tokenExistentes = await _context.FCM_TOKEN
                .Where(t => t.FCM_TOKEN == requestToken.FCM_TOKEN && t.APP == requestToken.APP).ToListAsync();

            var  tokenExistente = tokenExistentes.FirstOrDefault();
            if (tokenExistente != null)
            {
                if (esLogout)
                {
                    _context.FCM_TOKEN.Remove(tokenExistente);
                    _logger.LogInformation($"Token {requestToken.FCM_TOKEN} ELIMINADO físicamente por cierre de sesión.");
                }
                else
                {
                    tokenExistente.FCM_EMP_CODIGO = empleadoId.ToString();
                    tokenExistente.FCM_OS = requestToken.FCM_OS;
                    tokenExistente.MOD_FECHA = DateTime.Now;
                    tokenExistente.MOD_USR = usuario;

                    _context.FCM_TOKEN.Update(tokenExistente);
                    _logger.LogInformation($"Token {requestToken.APP} reasignado/actualizado al empleado: {empleadoId}");
                }
            }
            else
            {
                if (esLogout)
                {
                    _logger.LogInformation($"Se solicitó logout de un token inexistente. Ignorado.");
                }
                else if (empleadoId != 0) 
                {
                    // 3. INSERTAR EL NUEVO DISPOSITIVO
                    decimal fcm_codigo = await ObtenerSiguienteSecuencialAsync();

                    var nuevoRegistro = new Fcm_Token
                    {
                        FCM_EMP_CODIGO = empleadoId.ToString(),
                        FCM_TOKEN = requestToken.FCM_TOKEN, 
                        APP = requestToken.APP,
                        FCM_CODIGO = fcm_codigo,
                        FCM_OS = requestToken.FCM_OS,
                        CREA_FECHA = DateTime.Now,
                        CREA_USR = usuario
                    };

                    _context.FCM_TOKEN.Add(nuevoRegistro);
                    _logger.LogInformation($"Nuevo token insertado para el empleado: {empleadoId} | APP: {requestToken.APP}");
                }
            }

            await _context.SaveChangesAsync();

            _response.Data = requestToken.FCM_TOKEN;
            _response.Message = esLogout ? "Dispositivo desvinculado y eliminado correctamente." : "Aplicación y Token sincronizados exitosamente.";
            _response.Success = true;
        }
        catch (Exception ex)
        {
            _response.Message = "Error de servidor al registrar App: " + ex.Message;
            _response.Success = false;
            _logger.LogError(ex, "Error al ejecutar RegistrarAPPAsync.");
        }
        return _response;
    }

    private async Task<decimal> ObtenerSiguienteSecuencialAsync()
    {
        decimal fcm_codigo = 0;
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "SELECT FCM_TOKEN_SEQ.NEXTVAL FROM dual";
            await _context.Database.OpenConnectionAsync();
            try
            {
                var result = await command.ExecuteScalarAsync();
                fcm_codigo = Convert.ToDecimal(result);
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
        return fcm_codigo;
    }
    
    public async Task<ServiceResponse<object>> NotAppAgentesAsync()
    {
        var _response = new ServiceResponse<object>();
        DateTime FECHA = DateTime.Now.AddDays(-5);
        string nocontiene = "PLANIFICACIONES";
        int contador = 0;

        try
        {
            var listado = await (from n in _context.NOTIFICACIONES
                                    
                                 where n.APP_DESTINO == 1
                                    && n.NOT_INACTIVO == p_not_inactivo
                                    && n.NOT_PROCESADA == p_not_procesada
                                    && n.NOT_COMUNICADO != null
                                    && n.CREA_FECHA >= FECHA
                                    //&& !n.NOT_DESCRIPCION.ToUpper().Contains(nocontiene)
                                 join ng in _context.NOTIFICACIONES_GRUPOS
                                    on n.NOT_CODIGO equals ng.NOT_NOT_CODIGO
                                 join em in _context.EMPLEADO
                                    on ng.NOT_EMP_CODIGO equals em.EMP_CODIGO
                                 join fc in _context.FCM_TOKEN
                                    on em.EMP_CODIGO.ToString() equals fc.FCM_EMP_CODIGO
                                 where fc.APP == "APPAGENTES"

                                 select new
                                 {
                                     n.NOT_COMUNICADO,
                                     n.NOT_DESCRIPCION,
                                     fc.FCM_TOKEN,
                                     n.NOT_CODIGO
                                 }).ToListAsync();

            if (listado == null || !listado.Any())
            {
                _response.Message = "No existen notificaciones pendientes para AppAgentes.";
                _response.Success = true;
                _logger.LogInformation("NotAppAgentesAsync: " + _response.Message);
                return _response;
            }
            var accessToken = await GetAccessTokenPersonalizadoAsync(_AppAgentesJsonPath);


            foreach (var item in listado)
            {
                
                bool exito = await EnviarMensajeCoreAsync(item.FCM_TOKEN, item.NOT_COMUNICADO, item.NOT_DESCRIPCION, _AppAgentesProjectId, accessToken);
                if (exito)
                {
                    contador++;
                    Console.WriteLine($"Mensaje enviado a AppAgentes. Total: {contador}");
                }
            }
            var codigosUnicos = listado.Select(x => x.NOT_CODIGO).Distinct().ToList();

            if (codigosUnicos.Any())
            {
                var notificacionesAActualizar = await _context.NOTIFICACIONES
                    .Where(n => n.NOT_EMPRESA == p_empresa && codigosUnicos.Contains(n.NOT_CODIGO))
                    .ToListAsync();

                foreach (var notificacion in notificacionesAActualizar)
                {
                    notificacion.NOT_PROCESADA = 1;
                }

                await _context.SaveChangesAsync();
            }

            _response.Message = $"Proceso finalizado. Se enviaron {contador} notificaciones a AppAgentes.";
            _response.Success = true;
            _logger.LogInformation("Se ha ejecutado NotAppAgentesAsync correctamente.");
        }
        catch (Exception ex)
        {
            _response.Message = "Error en el servidor: " + ex.ToString();
            _response.Success = false;
            _logger.LogError(ex, "Error al ejecutar NotAppAgentesAsync.");
        }

        return _response;
    }

}