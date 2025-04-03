using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.SecurityNamespace;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Models.Firebase;
using PIGGISWS.Views.Marcaciones;

namespace PIGGISWS.Services;

public class MarcacionService: IMarcacionService
{

    private readonly ApplicationDbContext _context;
    private readonly ILogger<DevolucionesService> _logger;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();

    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;

    public MarcacionService(ApplicationDbContext context, ILogger<DevolucionesService> logger)
    {
        _context = context;
        _logger = logger;
        GetParametros();
    }

    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "DevolucionesService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }

    public async Task<ServiceResponse<object>> GetHoraAsync()
    {
        DateTime time;
        try
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT SYSDATE FROM dual";
                await _context.Database.OpenConnectionAsync();

                var result = await command.ExecuteScalarAsync();
                time = Convert.ToDateTime(result);
            }
            respuesta.Data = time;
            respuesta.Success = true;
            respuesta.Message = "Hora recuperada del servidor";
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Data = null;
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            _logger.LogError("ERROR AL RECUPERAR LA HORA: " + ex.ToString());
            return respuesta;
        }
    }

    public async Task<ServiceResponse<object>> CreateMarcacionAsync(Tmp_Marcacion_Agente marcacion_Agente)
    {
        decimal mar_codigo = 0;
        try
        {
           
            var registrosExistente = await _context.TMP_MARCACION_AGENTE
             .Where(x => x.AGE_CODIGO == marcacion_Agente.AGE_CODIGO
                                       && x.MAR_FECHA.Date == marcacion_Agente.MAR_FECHA.Date
                                       && x.ID_EMPRESA == marcacion_Agente.ID_EMPRESA).ToListAsync();
            var registroExistente = registrosExistente.FirstOrDefault();

            if (registroExistente == null)
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT DATA_USR.TMP_MARCACION_AGENTE_SEQ.NEXTVAL FROM dual";
                    await _context.Database.OpenConnectionAsync();

                    var result = await command.ExecuteScalarAsync();
                    mar_codigo = Convert.ToDecimal(result);
                }

                if (!string.IsNullOrEmpty(marcacion_Agente.ENTRADA1_MOVIL))
                {

                    marcacion_Agente.ENTRADA1 = DateTime.Now.ToString("HH:mm");
                    marcacion_Agente.ENTRADA1_MOVIL = DateTime.Parse(marcacion_Agente.ENTRADA1_MOVIL).ToString("HH:mm");

                }
                if (!string.IsNullOrEmpty(marcacion_Agente.SALIDA1_MOVIL))
                {

                    marcacion_Agente.SALIDA1 = DateTime.Now.ToString("HH:mm");
                    marcacion_Agente.SALIDA1_MOVIL = DateTime.Parse(marcacion_Agente.SALIDA1_MOVIL).ToString("HH:mm");

                }
                if (!string.IsNullOrEmpty(marcacion_Agente.ENTRADA2_MOVIL))
                {

                    marcacion_Agente.ENTRADA2 = DateTime.Now.ToString("HH:mm");
                    marcacion_Agente.ENTRADA2_MOVIL = DateTime.Parse(marcacion_Agente.ENTRADA2_MOVIL).ToString("HH:mm");
                }
                if (!string.IsNullOrEmpty(marcacion_Agente.SALIDA2_MOVIL))
                {
                   
                    marcacion_Agente.SALIDA2 = DateTime.Now.ToString("HH:mm:ss");
                    marcacion_Agente.SALIDA2_MOVIL = DateTime.Parse(marcacion_Agente.SALIDA2_MOVIL).ToString("HH:mm");
                }

                marcacion_Agente.ID_MARCACION = mar_codigo;
                marcacion_Agente.MAR_FECHA = marcacion_Agente.MAR_FECHA.Date;
                _context.TMP_MARCACION_AGENTE.Add(marcacion_Agente);
            }
            else
            {
                // Ya existe registro, actualizamos según los campos que tenga la nueva marcación.
                if (!string.IsNullOrEmpty(marcacion_Agente.ENTRADA1_MOVIL))
                {
                   
                    registroExistente.ENTRADA1_MOVIL = DateTime.Parse(marcacion_Agente.ENTRADA1_MOVIL).ToString("HH:mm");
                    registroExistente.ENTRADA1 = DateTime.Now.ToString("HH:mm");
                    registroExistente.UBICACION1 = marcacion_Agente.UBICACION1;
                }
                if (!string.IsNullOrEmpty(marcacion_Agente.SALIDA1_MOVIL))
                {
                    registroExistente.SALIDA1_MOVIL = DateTime.Parse(marcacion_Agente.SALIDA1_MOVIL).ToString("HH:mm");
                    registroExistente.SALIDA1 = DateTime.Now.ToString("HH:mm");
                    registroExistente.UBICACION2 = marcacion_Agente.UBICACION2;
                }
                if (!string.IsNullOrEmpty(marcacion_Agente.ENTRADA2_MOVIL))
                {
                    registroExistente.ENTRADA2_MOVIL = DateTime.Parse(marcacion_Agente.ENTRADA2_MOVIL).ToString("HH:mm");
                    registroExistente.ENTRADA2 = DateTime.Now.ToString("HH:mm");
                    registroExistente.UBICACION3 = marcacion_Agente.UBICACION3;
                }
                if (!string.IsNullOrEmpty(marcacion_Agente.SALIDA2_MOVIL))
                {
                    registroExistente.SALIDA2_MOVIL = DateTime.Parse(marcacion_Agente.SALIDA2_MOVIL).ToString("HH:mm");
                    registroExistente.SALIDA2 = DateTime.Now.ToString("HH:mm");
                    registroExistente.UBICACION4 = marcacion_Agente.UBICACION4;
                }

            }

            await _context.SaveChangesAsync();
            var ultimoRegistro = (registroExistente == null) ? marcacion_Agente : registroExistente;
            respuesta.Data = ultimoRegistro;
            respuesta.Success = true;
            respuesta.Message = "Marcación guardada correctamente.";
            return respuesta;
        }
        catch (Exception ex)
        {
            respuesta.Data = null;
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            _logger.LogError("ERROR AL GUARDAR MARCACIÓN: " + ex.ToString());
            return respuesta;
        }
    }

}
