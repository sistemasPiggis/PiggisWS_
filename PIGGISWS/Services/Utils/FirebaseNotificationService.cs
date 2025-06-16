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
using Microsoft.Graph;
using Azure;
using static Google.Apis.Requests.BatchRequest;
using PIGGISWS.Interfaces;

public class FirebaseNotificationService: IFirebaseNotificationService
{


    private readonly ApplicationDbContext _context;
    private const string FirebaseServerKey = "AAAA47TFP4g:APA91bGASDjXVLGZpGqRdMDAvLGna-tLziR1imYaKsLfQhTa75Zpw79c2qB-seXvBF05dIQ1y23JmLEy3oeTHyjKfC59HrbzekjOsqfB-7W7osr05OYmuDeuu4bwnuhY0QS6y7dGvJ-b";
    private const string FcmUrl = "https://fcm.googleapis.com/v1/projects/977990401928/messages:send";
    private readonly HttpClient _httpClient;
    private readonly ILogger<FirebaseNotificationService> _logger;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    
    int contador =0;
    int p_empresa;
    int p_not_inactivo;
    int p_not_procesada;

    // Constructor público para que el contenedor de dependencias pueda inyectarlo
    public FirebaseNotificationService(HttpClient httpClient , ApplicationDbContext context, ILogger<FirebaseNotificationService> logger)
    {
        _httpClient = httpClient;
        _context = context;
        GetParametros();
        _logger = logger;
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
        GoogleCredential credential;
        using (var stream = new FileStream("Firebase\\piggis-19039-firebase-adminsdk-z5ijt-5c3cce638c.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
        }

        var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        return token;
    }

    public async Task<ServiceResponse<object>> SendFcmMessageAsync(long not_codigo)
    {
        var _response = new ServiceResponse<object>();
        try
        {
            var listado = await (from n in _context.NOTIFICACIONES
                                 join ng in _context.NOTIFICACIONES_GRUPOS on n.NOT_CODIGO equals ng.NOT_NOT_CODIGO
                                 join em in _context.EMPLEADO on ng.NOT_EMP_CODIGO equals em.EMP_CODIGO
                                 join fc in _context.FCM_TOKEN on em.EMP_CODIGO equals fc.FCM_EMP_CODIGO
                                 where n.NOT_CODIGO == not_codigo
                                 && n.NOT_INACTIVO == p_not_inactivo
                                 && n.NOT_PROCESADA == p_not_procesada 
                                 select new
                                 {
                                     n.NOT_COMUNICADO,
                                     n.NOT_OBSERVACIONES,
                                     n.NOT_DESCRIPCION,
                                     fc.FCM_TOKEN
                                 }).ToListAsync();
            var accessToken = await GetAccessTokenAsync();

            if (listado == null || !listado.Any())
            {
                _response.Message = "No existe notificación en la base de datos o esta inactiva";

                _response.Success = true;
                return _response;
            }


            foreach (var item in listado)
            {

                var message = new
                {
                    message = new
                    {
                        token = item.FCM_TOKEN,
                        notification = new
                        {
                            title = item.NOT_COMUNICADO,
                            body = item.NOT_DESCRIPCION
                        }
                    }
                };


                var jsonMessage = JsonSerializer.Serialize(message);
                var httpContent = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                 var response = await _httpClient.PostAsync(FcmUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    contador++;
                    Console.WriteLine("Mensaje enviado correctamente." + contador);

                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al enviar el mensaje: {errorResponse}");
                }

            }
            var notificacion = _context.NOTIFICACIONES.Find( p_empresa,not_codigo);

            if (notificacion != null)
            {
                // Solo actualiza el campo específico
                notificacion.NOT_INACTIVO = 1;

                // Guarda los cambios
                _context.SaveChanges();
            }
            _response.Message = "Notificación enviada correctamente el número de notificaciones es:" + contador;
           
            _response.Success = true;
            return _response; 

        }
        catch (Exception ex)
        {
            _response.Message = ex.ToString();
            _response.Data = null;
            _response.Success = false;
        }
        return _response;
        }


    public async Task<ServiceResponse<object>> SendALLFcmMessageAsync()
    {
        DateTime FECHA = DateTime.Now.AddDays(-5);
        var _response = new ServiceResponse<object>();
        string nocontiene = "PLANIFICACIONES";
        try
        {
            var listado = await (from n in _context.NOTIFICACIONES
                                 join ng in _context.NOTIFICACIONES_GRUPOS on n.NOT_CODIGO equals ng.NOT_NOT_CODIGO
                                 join em in _context.EMPLEADO on ng.NOT_EMP_CODIGO equals em.EMP_CODIGO
                                 join fc in _context.FCM_TOKEN on em.EMP_CODIGO equals fc.FCM_EMP_CODIGO
                                 where  !n.NOT_DESCRIPCION.ToUpper().Contains(nocontiene) 
                                 && n.NOT_INACTIVO == p_not_inactivo
                                 && n.NOT_PROCESADA == p_not_procesada
                                 && n.NOT_COMUNICADO != null
                                 && n.CREA_FECHA >= FECHA
                                 select new
                                 {
                                     n.NOT_COMUNICADO,
                                     n.NOT_OBSERVACIONES,
                                     n.NOT_DESCRIPCION,
                                     fc.FCM_TOKEN, 
                                     n.NOT_CODIGO
                                 }).ToListAsync();
            var accessToken = await GetAccessTokenAsync();

            if (listado == null || !listado.Any())
            {
                _response.Message = "No existe notificación en la base de datos o esta inactiva";

                _response.Success = true;
                _logger.LogInformation("Se ha ejecutado SendALLFcmMessageAsync." + _response.Message);
                return _response;
            }


            foreach (var item in listado)
            {

                var message = new
                {
                    message = new
                    {
                        token = item.FCM_TOKEN,
                        notification = new
                        {
                            title = item.NOT_COMUNICADO,
                            body = item.NOT_DESCRIPCION
                        }
                    }
                };


                var jsonMessage = JsonSerializer.Serialize(message);
                var httpContent = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.PostAsync(FcmUrl, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    contador++;
                    Console.WriteLine("Mensaje enviado correctamente." + contador);

                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al enviar el mensaje: {errorResponse}");
                }
                var notificaciones = await _context.NOTIFICACIONES.Where(n =>n.NOT_EMPRESA == p_empresa && n.NOT_CODIGO == item.NOT_CODIGO).ToListAsync();
                var notificacion = notificaciones.FirstOrDefault();
                if (notificacion != null)
                {
                    // Solo actualiza el campo específico
                    notificacion.NOT_PROCESADA = 1;

                    // Guarda los cambios
                    _context.SaveChanges();
                }
            }
           
            _response.Message = "Notificación enviada correctamente el número de notificaciones es:" + contador;

            _response.Success = true;
            _logger.LogInformation("Se ha ejecutado SendALLFcmMessageAsync.");
            return _response;

        }
        catch (Exception ex)
        {
            _response.Message = ex.ToString();
            _response.Data = null;
            _response.Success = false;
            _logger.LogError(ex, "Error al ejecutar SendALLFcmMessageAsync.");
        }
        return _response;
    }
}




