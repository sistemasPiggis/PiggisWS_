using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using static Google.Apis.Requests.BatchRequest;

namespace PIGGISWS.Services;

public class DevolucionesService : IDevolucionesService
{

    private readonly ApplicationDbContext _context;
    private readonly ILogger<DevolucionesService> _logger;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();

    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;
    int p_car_siglafac;
    decimal CRT_NUMERO;

    public DevolucionesService(ApplicationDbContext context, ILogger<DevolucionesService> logger)
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
            p_car_siglafac = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 55)?.VALOR ?? "0");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }


    public async Task<ServiceResponse<object>> GetProDevxClienteAgeAsync(Cliente cliente)
    {
        var response = new ServiceResponse<object>();
        try
        {
            var result = await (
                                from rv in _context.REP_VENTAS_INT_60
                                join cl in _context.CLIENTE on rv.CCO_CODCLIPRO equals cl.CLI_CODIGO
                                where cl.CLI_AGENTE == cliente.CLI_AGENTE && rv.CCO_CODCLIPRO == cliente.CLI_CODIGO
                                select new
                                {
                                    PRO_NOMBRE = rv.PRO_ID + ". " + rv.PRO_NOMBRE,
                                    rv.PRO_CODIGO,
                                    rv.PRO_ID,
                                    rv.UMD_CODIGO,
                                    rv.UMD_ID,
                                    rv.CCO_CODCLIPRO
                                })
                                .Distinct()
                                .OrderBy(x => x.PRO_ID)
                                .ToListAsync();



            if (result == null || !result.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS PARA EL CLIENTE SELECCIONADO";
            }

            response.Data = result;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
        }
        return response;
    }


    public async Task<ServiceResponse<object>> GetProDevMotivoseAsync()
    {
        var response = new ServiceResponse<object>();
        try
        {
            var result = await (
                                from m in _context.REP_MOTIVOS_DEV
                                where m.TDE_EMPRESA == p_empresa && (m.TDE_INACTIVO ?? 0) == 0
                                select new
                                {
                                    TDE_NOMBRE = m.TDE_ID + ".- " + m.TDE_NOMBRE,
                                    TDE_CODIGO = m.TDE_CODIGO.ToString()
                                })
                                .ToListAsync();




            if (result == null || !result.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS PARA EL CLIENTE SELECCIONADO";
            }

            response.Data = result;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
        }
        return response;
    }

    public async Task<bool> ValidaDevolucion(Devolucion_Ext dev)
    {
        try
        {
            var result = await _context.DEVOLUCION_EXT.Where(d => d.DEV_SECUENCIAL_MOVIL == dev.DEV_SECUENCIAL_MOVIL &&
                                 d.DEV_REFERENCIA_UNICA_TX == dev.DEV_REFERENCIA_UNICA_TX).FirstOrDefaultAsync();
            if (result != null)
            {
                return true;
            }
            _logger.LogWarning("La devolución ya existe: " + dev.DEV_CODIGO + dev.DEV_REFERENCIA_UNICA_TX);
            return false;
        }

        catch (Exception ex)
        {
            _logger.LogWarning("exite un error al validar la devolucion: " + ex.ToString());
            return false;
        }
    }

    public async Task<ServiceResponse<object>> CreateDevolucionAsync(Devolucion_Ext devext, Devolucion_Cab devcab, List<Devolucion_Det> devolucion_Dets)
    {
        decimal dev_codigo = 0;
        
        if (await ValidaDevolucion(devext))
        {
            respuesta.Success = false;
            respuesta.Message = "La devolución ya existe";
            return respuesta;
        }
        else
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "SELECT DEVOLUCION_S_CODIGO.NEXTVAL FROM dual";
                        await _context.Database.OpenConnectionAsync();

                        var result = await command.ExecuteScalarAsync();
                        dev_codigo = Convert.ToDecimal(result);
                    }

                    var Ndev_ext = new Devolucion_Ext
                    {
                        DEV_CODIGO = dev_codigo,
                        DEV_EMPRESA = p_empresa,
                        DEV_REFERENCIA_UNICA_TX = devext.DEV_REFERENCIA_UNICA_TX,
                        DEV_SECUENCIAL_MOVIL = devext.DEV_SECUENCIAL_MOVIL,
                        DEV_LATITUD_NR = devext.DEV_LATITUD_NR,
                        DEV_LONGITUD_NR = devext.DEV_LONGITUD_NR,
                        DEV_FECHA_CREACION_ORG_DT = devext.DEV_FECHA_CREACION_ORG_DT
                    };
                    await _context.DEVOLUCION_EXT.AddAsync(Ndev_ext);
                    int extsave = await _context.SaveChangesAsync();
                    if (extsave == 0)
                    {
                        transaction.Rollback();
                        respuesta.Data = null;
                        respuesta.Success = false;
                        respuesta.Message = "ERROR AL GUARDAR DEVOLUCION_EXT";
                        _logger.LogError("Error al guardar en DEVOLUCION_EXT: {DEV_CODIGO}", dev_codigo);
                        return respuesta;
                    }
                    _logger.LogInformation("Guardado en DEVOLUCION_EXT: {DEV_CODIGO}", dev_codigo);
                    var NDev_cabecera = new Devolucion_Cab
                    {
                        DEV_NUMERO = devcab.DEV_NUMERO,
                        DEV_CLIENTE = devcab.DEV_CLIENTE,
                        DEV_AGENTE = devcab.DEV_AGENTE,
                        DEV_FECHA = devcab.DEV_FECHA,
                        DEV_OBSERVACION = devcab.DEV_OBSERVACION,
                        DEV_NUM_FUNDAS = devcab.DEV_NUM_FUNDAS,
                        DEV_IMPRESO = devcab.DEV_IMPRESO,
                        DEV_DOC_REFERENCIA = devcab.DEV_DOC_REFERENCIA,
                        DEV_ESTADO = devcab.DEV_ESTADO,
                        DEV_OBSERVACION_VALIDADOR = devcab.DEV_OBSERVACION_VALIDADOR
                    };
                    await _context.DEVOLUCION_CAB.AddAsync(NDev_cabecera);
                    int cabsave = await _context.SaveChangesAsync();
                    if (cabsave == 0)
                    {

                        transaction.Rollback();
                        respuesta.Data = null;
                        respuesta.Success = false;
                        respuesta.Message = "ERROR AL GUARDAR DEVOLUCION CABECERA.";
                        _logger.LogError("Error al guardar en DEVOLUCION_CAB: {DEV_CODIGO}", dev_codigo);
                        return respuesta;

                    }
                    _logger.LogInformation("Guardado en DEVOLUCION_CAB: {DEV_CODIGO}", dev_codigo);
                    foreach (var item in devolucion_Dets)
                    {
                        var NDev_detalle = new Devolucion_Det
                        {
                            DVD_CODIGO = dev_codigo,
                            DVD_PRODUCTO = item.DVD_PRODUCTO,
                            DVD_UNIDAD = item.DVD_UNIDAD,
                            DVD_CANTIDAD = item.DVD_CANTIDAD,
                            DVD_REFERENCIA = item.DVD_REFERENCIA,
                            DVD_FACTURA = item.DVD_FACTURA,
                            DVD_PROCESA = item.DVD_PROCESA,
                            DVD_OBSERVACION = item.DVD_OBSERVACION,
                            DVD_MOTIVO = item.DVD_MOTIVO,
                            DVD_SECUENCIA = item.DVD_SECUENCIA,
                            DVD_CARGADO_DESDE = item.DVD_CARGADO_DESDE,
                            DVD_ESTADO = item.DVD_ESTADO
                        };
                        await _context.DEVOLUCION_DET.AddAsync(NDev_detalle);
                    }
                    int detsave = await _context.SaveChangesAsync();
                    if (detsave == 0)
                    {
                        transaction.Rollback();
                        respuesta.Data = null;
                        respuesta.Success = false;
                        respuesta.Message = "EXISTIO UN PROBLEMA AL GUARDAR DEVOLUCION DETALLE";
                        _logger.LogError("Error al guardar en DEVOLUCION_DET: {DEV_CODIGO}", dev_codigo);
                        return respuesta;
                    }

                    await transaction.CommitAsync();
                    respuesta.Data = new { NDev_cabecera };
                    respuesta.Success = true;
                    respuesta.Message = "Pedido guardado exitosamente # de pedido = " + NDev_cabecera.DEV_NUMERO;
                    _logger.LogInformation("Transacción completada exitosamente: {DEV_CODIGO}", dev_codigo);
                    return respuesta;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    respuesta.Data = null;
                    respuesta.Success = false;
                    respuesta.Message = ex.ToString();
                    _logger.LogError("Error al guardar devolucion:" + ex.ToString() );
                    return respuesta;
                }
            }
        }
    }
}
