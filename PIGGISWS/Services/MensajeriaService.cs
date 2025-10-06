using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Services;

public class MensajeriaService :IMensajeriaService

{
    private readonly ApplicationDbContext _context;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    private readonly ILogger<MensajeriaService> _logger;
    int p_empresa = 0;
    string p_nav_lista_destinatarios = string.Empty;
    string p_nav_modulo = string.Empty;
    int p_nav_envia_sms ;
    int p_nav_envia_correo;
    string p_nav_mensaje_correo = string.Empty;
    string p_nav_asunto_correo =string.Empty;
    int id_mensaje;
    public MensajeriaService(ApplicationDbContext context, ILogger<MensajeriaService> logger)
    {
        _context = context;
        _logger = logger;
        GetParametros();
    }
    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "MensajeriaService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
            p_nav_lista_destinatarios = parametros.FirstOrDefault(p => p.CODIGO == 62)?.VALOR ?? "0";
            p_nav_modulo = parametros.FirstOrDefault(p => p.CODIGO == 63)?.VALOR ?? "0";
            p_nav_envia_sms = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 64)?.VALOR ?? "0");
            p_nav_envia_correo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 65)?.VALOR ?? "0");
            p_nav_mensaje_correo = parametros.FirstOrDefault(p => p.CODIGO == 66)?.VALOR ?? "0";
            p_nav_asunto_correo = parametros.FirstOrDefault(p => p.CODIGO == 67)?.VALOR ?? "0";
        }
        catch (Exception ex)
        {
            _logger.LogError(" --------------------- ERROR ------------------ GetParametros mensajeria " + ex.ToString());
        }
    }
    public async Task<ServiceResponse<object>> CreateMensajeNavAsync(AuxGeneral aux)
    {
        var response = new ServiceResponse<object>();

        try
        {
             
            var agente = await (from p in _context.TDS_PEDIDOS_NAV_CAB 
                                join c in _context.CLIENTE on p.CLI_CODIGO equals c.CLI_CODIGO
                                join a in _context.AGENTE on c.CLI_AGENTE equals a.AGE_CODIGO
                                where p.ID_PEDIDO_NAV == aux.AuxDecimal
                                select new
                                {
                                    a.AGE_NOMBRE
                                }).ToListAsync();



            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT DATA_USR.GEN_MENSAJERIA_SEQ.nextval FROM dual";
                await _context.Database.OpenConnectionAsync();

                var result = await command.ExecuteScalarAsync();
                id_mensaje = Convert.ToInt32(result);
            }


            string link = $"<a href=\"http://190.12.5.82:8025/utilidades/printPN.php?CCO_EMPRESA=1&CCO_CODIGO={aux.AuxDecimal}\">Ver Pedido</a>";

            var mensaje = new GEN_MENSAJERIA
            { 
                ID_MENSAJE = id_mensaje,
                DESTINO_SMS = null,
                DESTINO_CORREO = p_nav_lista_destinatarios,
                MENSAJE_SMS = null,
                ESTADO_SMS = 0,
                ESTADO_CORREO = 0,
                MODULO = p_nav_modulo,
                ENVIA_SMS = p_nav_envia_sms,
                ENVIA_CORREO = p_nav_envia_correo,
                MENSAJE_CORREO = p_nav_mensaje_correo + " " + aux.AuxDecimal + " DEL AGENTE " 
                                 + (agente.FirstOrDefault()?.AGE_NOMBRE ?? "CLIENTE SIN AGENTE ASIGNADO") +
                                " " + link,
                ASUNTO_CORREO = p_nav_asunto_correo + " " + aux.AuxDecimal + " DEL AGENTE "
                                 + agente.FirstOrDefault()?.AGE_NOMBRE ?? "CLIENTE SIN AGENTE ASIGNADO",
                QUIEN_ENVIA_CORREO = agente.FirstOrDefault()?.AGE_NOMBRE ?? "CLIENTE SIN AGENTE ASIGNADO",
                RESPONDER_CORREO_A = null, 
                //CREA_USR = "WS",
                CREA_FECHA = DateTime.Now,
                //MOD_USR = "WS",
                MOD_FECHA = DateTime.Now, 
                PROCESO_ENVIO = "CLASICO" 
            };


            await _context.GEN_MENSAJERIA.AddAsync(mensaje);

             await _context.SaveChangesAsync();
     


                        response = new ServiceResponse<object>
            {
                Data = "MENSAJE ENVIADO",
                Success = true,
                Message = "Mensaje creado correctamente." + mensaje.ID_MENSAJE,

            };

        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al procesar la solicitud: {ex.Message}";
        }
        return response;
    }


}
