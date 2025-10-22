using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Oracle.ManagedDataAccess.Client;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Services.Utils;
using System;
using System.Globalization;
using System.Linq;


namespace PIGGISWS.Services;

public class DevolucionesService : IDevolucionesService
{

    private readonly ApplicationDbContext _context;
    private readonly ILogger<DevolucionesService> _logger;
    private readonly ICarteraService _carteraService;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();

    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;
    int p_car_siglafac;
    decimal CRT_NUMERO;
    string p_estado_dev ="";

    public DevolucionesService(ApplicationDbContext context, ILogger<DevolucionesService> logger, ICarteraService carteraService)
    {
        _context = context;
        _logger = logger;
        GetParametros();
        _carteraService = carteraService;
    }

    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "DevolucionesService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
            p_car_siglafac = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 55)?.VALOR ?? "0");
            p_estado_dev = parametros.FirstOrDefault(p => p.CODIGO == 56)?.VALOR ?? "G";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            _logger.LogError(" --------------------- ERROR ------------------ GetParametros() " + ex.ToString());
        }

    }


    public async Task<ServiceResponse<object>> GetProDevxClienteAgeAsync(Cliente cliente)
    {
        var response = new ServiceResponse<object>();
        try
        {
            var result = await (
                                from rv in _context.REP_VENTAS_INT_PARAM
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
            _logger.LogError(" --------------------- ERROR ------------------ GetProDevxClienteAgeAsync() " + ex.ToString() + cliente);
        }
        return response;
    }


    public async Task<ServiceResponse<object>> GetProDevMotivoseAsync()
    {
        var response = new ServiceResponse<object>(); 
        try
        {
            var result = await (
                                from m in _context.REP_MOTIVOS_DEV_APP
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
            _logger.LogError(" --------------------- ERROR ------------------ GetProDevMotivoseAsync() " + ex.ToString());
        }
        return response;
    }

    public async Task<bool> ValidaDevolucion(Devolucion_Ext dev)
    {
        try
        {
            var results = await _context.DEVOLUCION_EXT.Where(d => d.DEV_SECUENCIAL_MOVIL == dev.DEV_SECUENCIAL_MOVIL 
                                 ).ToListAsync();
            var result = results.FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            _logger.LogWarning("La devolución ya existe elimine, actualice los datos y vuelva a crear: " + dev.DEV_CODIGO + dev.DEV_REFERENCIA_UNICA_TX);
            return false;
        }

        catch (Exception ex)
        {
            _logger.LogError(" --------------------- ERROR ------------------ ValidaDevolucion() " + ex.ToString() + dev);
            return false;
        }
    }

    public async Task<ServiceResponse<object>> CreateDevolucionAsync(AuxDevolucion auxDevolucion)
    {
        decimal dev_codigo = 0;
        DateTime fechadev = auxDevolucion.Cabecera?.DEV_FECHA ?? DateTime.Now;

        if (auxDevolucion == null || auxDevolucion.Ext == null || auxDevolucion.Cabecera == null || auxDevolucion.Detalle == null )
        {
            if(auxDevolucion?.Ext?.DEV_SECUENCIAL_MOVIL == 0 || auxDevolucion?.Ext?.DEV_SECUENCIAL_MOVIL == null)
            {
                respuesta.Data = null;
                respuesta.Success = false;
                respuesta.Message = "EL SECUENCIAL NO ES EL CORRECTO POR FAVOR SINCRONIZAR DATOS";
                _logger.LogError("Error al guardar devolución: No todos los datos están completos");
                return respuesta;
            }

            respuesta.Data = null;
            respuesta.Success = false;
            respuesta.Message = "No todos los datos están completos, por favor actualizar todos los datos";
            _logger.LogError("Error al guardar devolución: No todos los datos están completos");
            return respuesta;
        }
 
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "SELECT DATA_USR.DEVOLUCION_S_CODIGO.NEXTVAL FROM dual";
                        await _context.Database.OpenConnectionAsync();

                        var result = await command.ExecuteScalarAsync();
                        dev_codigo = Convert.ToDecimal(result);
                    }

                    //var secuenciales = await (from ext in _context.DEVOLUCION_EXT
                    //                          join ca in _context.DEVOLUCION_CAB on ext.DEV_CODIGO equals ca.DEV_CODIGO
                    //                          where ca.DEV_AGENTE == auxDevolucion.Cabecera.DEV_AGENTE
                    //                          orderby ext.DEV_SECUENCIAL_MOVIL descending
                    //                          select ext.DEV_SECUENCIAL_MOVIL).ToListAsync();
                    //decimal ultimosec = Convert.ToInt16(secuenciales.FirstOrDefault());
                    //ultimosec++;

                    var Ndev_ext = new Devolucion_Ext
                    {
                        DEV_CODIGO = dev_codigo,
                        DEV_EMPRESA = p_empresa,
                        DEV_REFERENCIA_UNICA_TX = auxDevolucion.Ext.DEV_REFERENCIA_UNICA_TX,
                        DEV_SECUENCIAL_MOVIL = auxDevolucion.Ext.DEV_SECUENCIAL_MOVIL,
                        DEV_LATITUD_NR = auxDevolucion.Ext.DEV_LATITUD_NR,
                        DEV_LONGITUD_NR = auxDevolucion.Ext.DEV_LONGITUD_NR,
                        DEV_FECHA_CREACION_ORG_DT = fechadev.Date
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

                    string devAgenteStr = auxDevolucion.Cabecera.DEV_AGENTE?.ToString("F0") ?? "0";
                    string devNumeroStr = $"{devAgenteStr}{auxDevolucion.Ext.DEV_SECUENCIAL_MOVIL}";
                    decimal devNumero = Convert.ToDecimal(devNumeroStr);

                    var NDev_cabecera = new Devolucion_Cab
                    {
                        DEV_CODIGO = dev_codigo,
                        DEV_NUMERO = devNumero,
                        DEV_CLIENTE = auxDevolucion.Cabecera.DEV_CLIENTE,
                        DEV_AGENTE = auxDevolucion.Cabecera.DEV_AGENTE,
                        DEV_FECHA = fechadev.Date,
                        DEV_OBSERVACION = auxDevolucion.Cabecera.DEV_OBSERVACION,
                        DEV_NUM_FUNDAS = auxDevolucion.Cabecera.DEV_NUM_FUNDAS,
                        DEV_IMPRESO = 0,
                        DEV_ESTADO = p_estado_dev,

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
                    foreach (var item in auxDevolucion.Detalle!)
                    {
                        var NDev_detalle = new Devolucion_Det
                        {
                            DVD_CODIGO = NDev_cabecera.DEV_CODIGO,
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
                        respuesta.Message = "EXISTIÓ UN PROBLEMA AL GUARDAR DEVOLUCION DETALLE";
                        _logger.LogError("Error al guardar en DEVOLUCION_DET: {DEV_CODIGO}", dev_codigo);
                        return respuesta;
                    }

                    await transaction.CommitAsync();
                    AuxDevolucion respauxdev = new AuxDevolucion
                    {
                        Cabecera = NDev_cabecera,
                        Detalle = null,
                        Ext = null
                    };
                    respuesta.Data =  respauxdev ;
                    respuesta.Success = true;
                    respuesta.Message = "Pedido guardado exitosamente # de DEVOLUCION = " + respauxdev.Cabecera.DEV_NUMERO;

                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "BEGIN DATA_USR.P_ENVIA_CORREO_DEV(:DVD_CODIGO, :DEV_CLIENTE); END;";
                        command.CommandType = System.Data.CommandType.Text;

                        // Agrega los parámetros
                        var paramCodigo = new OracleParameter("DVD_CODIGO", dev_codigo); 
                        var paramCliente = new OracleParameter("DEV_CLIENTE", auxDevolucion.Cabecera.DEV_CLIENTE); 

                        command.Parameters.Add(paramCodigo);
                        command.Parameters.Add(paramCliente);

                        if (command.Connection.State != System.Data.ConnectionState.Open)
                            await command.Connection.OpenAsync();

                        await command.ExecuteNonQueryAsync();
                    }


                    _logger.LogInformation("Transacción completada exitosamente: {DEV_CODIGO}", dev_codigo);
                    return respuesta;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    respuesta.Data = null;
                    respuesta.Success = false;
                    respuesta.Message = ex.ToString();
                    _logger.LogError("---------------------------------ERROR AL GUARDAR DEVOLUCION: ----------------------" + ex.ToString() );
                    return respuesta;
                }
            }
        
    }



    public async Task<ServiceResponse<object>> GetProDevsxAgeAsync(decimal agente)
    {
        System.DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

        // Crea un objeto CultureInfo en español
        CultureInfo ci = new CultureInfo("es-ES");

        // Obtiene el nombre del día de la semana en español
        string dayName = ci.DateTimeFormat.GetDayName(dayOfWeek);
        //var diasPermitidos = new[] { "VIERNES", "DOMINGO" };
        var diasPermitidos = new[] { "DOMINGO" };
        string dayformateado = dayName.ToUpper();
        dayformateado = FormatosTexto.RemoveDiacritics(dayformateado);
        var response = new ServiceResponse<object>();
        try
        {
            var result = await (
                                from rv in _context.REP_VENTAS_INT_PARAM
                                join cl in _context.CLIENTE on rv.CCO_CODCLIPRO equals cl.CLI_CODIGO
                                join cd in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd.CDI_CLIENTE
                                where cl.CLI_AGENTE == agente && cl.CLI_INACTIVO ==0
 && (
                                            diasPermitidos.Contains(dayformateado)
                                            || (cd.CDI_DIA != null && cd.CDI_DIA == dayformateado)
                                        )
                                select new
                                {
                                    PRO_NOMBRE = rv.PRO_ID + ". " + rv.PRO_NOMBRE,
                                    rv.PRO_CODIGO,
                                    rv.PRO_ID,
                                    rv.UMD_CODIGO,
                                    rv.UMD_ID,
                                    rv.CCO_CODCLIPRO, 

                                    rv .FAC_CANTIDAD_ORIGINAL
                                })
                                .Distinct()
                                .OrderBy(x => x.PRO_ID)
                                .ToListAsync();



            if (result == null || !result.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS";
                return response;
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

            _logger.LogError(" --------------------- ERROR ------------------ GetProDevsxAgeAsync() " + ex.ToString() + agente);
        }
        return response;
    }


    public async Task<ServiceResponse<object>> GetSecuenciaDevAsync(decimal agente)
    {
        var response = new ServiceResponse<object>();
        try
        {


            var secuenciales = await (from ext in _context.DEVOLUCION_EXT
                                      join ca in _context.DEVOLUCION_CAB on ext.DEV_CODIGO equals ca.DEV_CODIGO
                                      where ca.DEV_AGENTE == agente && ext.DEV_SECUENCIAL_MOVIL != 0 
                                      && ext.DEV_SECUENCIAL_MOVIL != null
                                      orderby ext.DEV_SECUENCIAL_MOVIL descending
                                      select ext.DEV_SECUENCIAL_MOVIL).ToListAsync();
            decimal ultimosec = Convert.ToDecimal(secuenciales.FirstOrDefault());
            if (ultimosec == 0)
            {
                response.Data = 0;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS";
                return response;
            }

            response.Data = ultimosec;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetSecuenciaDevAsync() " + ex.ToString() + agente);
            return response;
        }
    }



    public async Task<ServiceResponse<object>> GetDevsxAgeAsync(decimal agente)
    {
       
        var response = new ServiceResponse<object>();
        try
        {

            int currentYear = DateTime.Now.Year;
            var query1 = from d in _context.TMP_DEV_PROD_CLI_AGE
                         where d.AGENTE_FK == agente && d.ID_DEVOLUCION_NR != null
                         && d.FECHA_DT.HasValue
                            && d.FECHA_DT.Value.Year == currentYear
                         group d by new { d.ID_DEVOLUCION_NR, d.FECHA_DT, d.NUMERO_IDC } into g
                         select new
                         {
                             CODIGO = g.Key.ID_DEVOLUCION_NR,
                             NUMERO = g.Key.NUMERO_IDC,
                             FECHA = (DateTime?)g.Key.FECHA_DT,
                             SOURCE = "A"
                         };

            var query2 = from c in _context.DEVOLUCION_CAB
                         where c.DEV_AGENTE == agente
                         && c.DEV_FECHA.HasValue
                          && c.DEV_FECHA.Value.Year == currentYear
                         select new
                         {
                             CODIGO = c.DEV_CODIGO,
                             NUMERO = c.DEV_NUMERO,
                             FECHA = c.DEV_FECHA,
                             SOURCE = "N"
                         };

            var unionQuery = query1.Union(query2);

            var data = await unionQuery.ToListAsync();

            var resultados = data
                .Select(x => new
                {
                    ZON_NOMBRE = (x.FECHA.HasValue ? x.FECHA.Value.ToString("yyyy-MM-dd") : "") + " - " + x.NUMERO.ToString(),
                    CRT_DOCTRAN = x.CODIGO.ToString() + x.SOURCE
                })
                //.OrderByDescending(x => DateTime.TryParse(x.ZON_NOMBRE.Split(" - ")[0], out var fecha) ? fecha : DateTime.MinValue)
                //.ThenByDescending(x => x.ZON_NOMBRE)
                .ToList();



            if (resultados == null || !resultados.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS";
            }

            response.Data = resultados;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetDevsxAgeAsync() " + ex.ToString() + agente);
        }
        return response;
    }


    public async Task<ServiceResponse<object>> GetDevxCodigoAsync(AuxGeneral dev)

    {

        var response = new ServiceResponse<object>();
        try
        {


            var query1 = from D in _context.TMP_DEV_PROD_CLI_AGE
                         join C in _context.CLIENTE on D.CLIENTE_FK equals C.CLI_CODIGO
                         join A in _context.AGENTE on D.AGENTE_FK equals A.AGE_CODIGO
                         join P in _context.PRODUCTO on D.PRODUCTO_FK equals P.PRO_CODIGO
                         join U in _context.UMEDIDA on P.PRO_UNIDAD equals U.UMD_CODIGO
                         join RV in _context.REP_MOTIVOS_DEV_APP on D.MOTIVO_FK equals RV.TDE_CODIGO
                         where D.AGENTE_FK == dev.AuxDecimal //// trae el agente
                            && (D.ID_DEVOLUCION_NR.ToString() + "A") == dev.AuxString // 
                         select new
                         {
                             PRO_ID = P.PRO_ID,
                             PRO_NOMBRE = P.PRO_NOMBRE,
                             UMD_ID = U.UMD_ID,
                             DMO_CDIGITADA = D.CANTIDAD_NR,
                             MOTIVO_FK = D.MOTIVO_FK,
                             TDE_NOMBRE = RV.TDE_NOMBRE
                         };

            var query2 = from DC in _context.DEVOLUCION_CAB
                         join D in _context.DEVOLUCION_DET on DC.DEV_CODIGO equals D.DVD_CODIGO
                         join P in _context.PRODUCTO on D.DVD_PRODUCTO equals P.PRO_CODIGO
                         join C in _context.CLIENTE on DC.DEV_CLIENTE equals C.CLI_CODIGO
                         join A in _context.AGENTE on DC.DEV_AGENTE equals A.AGE_CODIGO
                         join U in _context.UMEDIDA on D.DVD_UNIDAD equals U.UMD_CODIGO
                         join RV in _context.REP_MOTIVOS_DEV_APP on D.DVD_MOTIVO equals RV.TDE_CODIGO into RVLeft
                         from RV in RVLeft.DefaultIfEmpty()
                         where (D.DVD_CODIGO.ToString() + "N") == Convert.ToString(dev.AuxString)
                         select new
                         {
                             PRO_ID = P.PRO_ID,
                             PRO_NOMBRE = P.PRO_NOMBRE,
                             UMD_ID = U.UMD_ID,
                             DMO_CDIGITADA = D.DVD_CANTIDAD,
                             MOTIVO_FK = D.DVD_MOTIVO,
                             TDE_NOMBRE = RV != null ? RV.TDE_NOMBRE : null
                         };

            var resultados = await query1
                .Union(query2)
                .ToListAsync();


            if (resultados == null || !resultados.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS";
                return response;
            }

            response.Data = resultados;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetDevxCodigoAsync() " + ex.ToString() + dev);
        }
        return response;


    }

    public async Task<ServiceResponse<object>> GetDevDetxCodigoAsync(AuxGeneral dev)

    {

        var response = new ServiceResponse<object>();
        try
        {



            // Primer query: TMP_DEV_PROD_CLI_AGE
            var query1 = from d in _context.TMP_DEV_PROD_CLI_AGE
                         join c in _context.CLIENTE on d.CLIENTE_FK equals c.CLI_CODIGO
                         join a in _context.AGENTE on d.AGENTE_FK equals a.AGE_CODIGO
                         where d.AGENTE_FK == dev.AGE_CODIGO
                            && (d.ID_DEVOLUCION_NR.ToString() + "A") == dev.AuxString
                         group d by new { d.NUMERO_IDC, a.AGE_NOMBRE, c.CLI_NOMBRE, d.FECHA_DT, d.ESTADO_TX } into g
                         select new
                         {
                             NUMERO_IDC = g.Key.NUMERO_IDC,
                             CLI_NOMBRE = g.Key.CLI_NOMBRE,
                             FECHA_DT = g.Key.FECHA_DT,
                             ESTADO_TX = g.Key.ESTADO_TX
                         };

            // Segundo query: DEVOLUCION_CAB
            var query2 = from d in _context.DEVOLUCION_CAB
                         join c in _context.CLIENTE on d.DEV_CLIENTE equals c.CLI_CODIGO
                         join a in _context.AGENTE on d.DEV_AGENTE equals a.AGE_CODIGO
                         where d.DEV_AGENTE == dev.AGE_CODIGO
                            && (d.DEV_CODIGO.ToString() + "N") == dev.AuxString
                         select new
                         {
                             NUMERO_IDC = d.DEV_DOC_REFERENCIA,
                             CLI_NOMBRE = c.CLI_NOMBRE,
                             FECHA_DT = d.DEV_FECHA,
                             ESTADO_TX = d.DEV_ESTADO
                         };

            // Ejecuta ambas consultas y une los resultados en memoria
            var list1 = await query1.ToListAsync();
            var list2 = await query2.ToListAsync();

            var unionList = (await Task.WhenAll(
                    list1.Select(async x => new
                 {
                     NUMERO_COMPROBANTE = x.NUMERO_IDC != null ? await _carteraService.ObtenerNumeroComprobanteAsync(1, x.NUMERO_IDC ?? 0) : null,
                     NUMERO_IDC = x.NUMERO_IDC ?? 0,
                     CLI_NOMBRE = x.CLI_NOMBRE,
                     FECHA_DT = x.FECHA_DT,
                     ESTADO = x.ESTADO_TX == "I" ? "INGRESADO" :
                              x.ESTADO_TX == "G" ? "GRABADO" :
                              x.ESTADO_TX == "P" ? "PROCESADO" : ""
                 })
             )).ToList();

            unionList.AddRange(
                await Task.WhenAll(list2.Select(async x => new
                {
                    NUMERO_COMPROBANTE = x.NUMERO_IDC != null ? await _carteraService.ObtenerNumeroComprobanteAsync(1, x.NUMERO_IDC ?? 0) : null,
                    NUMERO_IDC = x.NUMERO_IDC ?? 0,
                    CLI_NOMBRE = x.CLI_NOMBRE,
                    FECHA_DT = x.FECHA_DT,
                    ESTADO = x.ESTADO_TX == "I" ? "INGRESADO" :
                             x.ESTADO_TX == "G" ? "GRABADO" :
                             x.ESTADO_TX == "P" ? "PROCESADO" : ""
                }))
            );

            // Si quieres ordenar por fecha descendente:
            var resultados = unionList.FirstOrDefault();


            if (resultados == null )
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS";
                return response;
            }

            response.Data = resultados;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetDevDetxCodigoAsync() " + ex.ToString() + dev);
        }
        return response;


    }



    public async Task<ServiceResponse<object>> GetDevProdApCodigoAsync(AuxGeneral dev)

    {

        var response = new ServiceResponse<object>();
        try
        {

            var resultados = await (
                from c in _context.CCOMPROBA
                join d in _context.DMOVINV on c.CCO_CODIGO equals d.DMO_CMO_COMPROBA
                join p in _context.PRODUCTO on d.DMO_PRODUCTO equals p.PRO_CODIGO
                join u in _context.UMEDIDA on d.DMO_UDIGITADA equals u.UMD_CODIGO
                join t in _context.TIPODEV on d.DMO_TIPODEV equals t.TDE_CODIGO
                where c.CCO_CODIGO == dev.AuxDecimal
                orderby d.DMO_SECUENCIA ascending
                select new
                {
                    PRO_ID = p.PRO_ID,
                    PRO_NOMBRE = p.PRO_NOMBRE,
                    UMD_ID = u.UMD_ID,
                    DMO_CDIGITADA = d.DMO_CDIGITADA,
                    TDE_NOMBRE = t.TDE_NOMBRE
                }
            ).ToListAsync();


            if (resultados == null)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS";
                return response;
            }

            response.Data = resultados;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetDevProdApCodigoAsync() " + ex.ToString() + dev);
        }
        return response;


    }




    public async Task<ServiceResponse<object>> GetDevProDenCodigoAsync(AuxGeneral dev)

    {

        var response = new ServiceResponse<object>();
        try
        {

            var resultados = await (
                 from c in _context.CCOMPROBA
                 join d in _context.DMOVINVI on c.CCO_CODIGO equals d.DMO_CMO_COMPROBA
                 join p in _context.PRODUCTO on d.DMO_PRODUCTO equals p.PRO_CODIGO
                 join u in _context.UMEDIDA on d.DMO_UDIGITADA equals u.UMD_CODIGO
                 join t in _context.TIPODEV on d.DMO_TIPODEV equals t.TDE_CODIGO
                 where c.CCO_CODIGO == dev.AuxDecimal
                 orderby d.DMO_SECUENCIA ascending
                 select new
                 {
                     PRO_ID = p.PRO_ID,
                     PRO_NOMBRE = p.PRO_NOMBRE,
                     UMD_ID = u.UMD_ID,
                     DMO_CDIGITADA = d.DMO_CDIGITADA,
                     TDE_NOMBRE = t.TDE_NOMBRE
                 }
             ).ToListAsync();


            if (resultados == null)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS";
                return response;
            }

            response.Data = resultados;
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetDevProDenCodigoAsync() " + ex.ToString() + dev);
        }
        return response;


    }

}
