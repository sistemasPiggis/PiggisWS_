using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Oracle.ManagedDataAccess.Client;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Models.Vistas;
using PIGGISWS.Services.Utils;
using System;
using System.Globalization;
using System.Linq;
using static Google.Apis.Requests.BatchRequest;


namespace PIGGISWS.Services;

public class DevolucionesService : IDevolucionesService
{

    private readonly ApplicationDbContext _context;
    private readonly ILogger<DevolucionesService> _logger;
    private readonly ICarteraService _carteraService;
    private readonly IClientesService _clientesService;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();

    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;
    int p_car_siglafac;
    decimal CRT_NUMERO;
    string p_estado_dev = "";
    int P_DIAS_IDVNOGEST;
    string P_DIAS_PERMITIDOS = "";

    public DevolucionesService(ApplicationDbContext context, ILogger<DevolucionesService> logger, ICarteraService carteraService, IClientesService clientesService)
    {
        _context = context;
        _logger = logger;
        GetParametros();
        _carteraService = carteraService;
        _clientesService = clientesService;
    }

    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "DevolucionesService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
            p_car_siglafac = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 55)?.VALOR ?? "0");
            p_estado_dev = parametros.FirstOrDefault(p => p.CODIGO == 56)?.VALOR ?? "G";
            P_DIAS_IDVNOGEST = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 69)?.VALOR ?? "0");
            P_DIAS_PERMITIDOS = parametros.First(p => p.CODIGO == 70)?.VALOR ?? "";
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

        if (auxDevolucion == null || auxDevolucion.Ext == null || auxDevolucion.Cabecera == null || auxDevolucion.Detalle == null)
        {
            if (auxDevolucion?.Ext?.DEV_SECUENCIAL_MOVIL == 0 || auxDevolucion?.Ext?.DEV_SECUENCIAL_MOVIL == null)
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


        // VALIDA QUE LA DEVOLUCIÓN SE HAYA CREADO 
        var existingDevExts = await _context.DEVOLUCION_EXT
        .Where(ext => ext.DEV_REFERENCIA_UNICA_TX == auxDevolucion.Ext.DEV_REFERENCIA_UNICA_TX).ToListAsync();


        var existingDevExt = existingDevExts.FirstOrDefault();
        if (existingDevExt != null)
        {

            var existeDevCabs = await _context.DEVOLUCION_CAB
                .Where(cab => cab.DEV_CODIGO == existingDevExt.DEV_CODIGO).ToListAsync();
            var existeDevCab = existeDevCabs.FirstOrDefault();
            var existeDevDet = await _context.DEVOLUCION_DET
               .Where(det => det.DVD_CODIGO == existingDevExt.DEV_CODIGO).ToListAsync();



            _logger.LogInformation("Devolución duplicada detectada con DEV_REFERENCIA_UNICA_TX: {Referencia}. Se retorna la devolución existente.", auxDevolucion.Ext.DEV_REFERENCIA_UNICA_TX);


            respuesta.Data = new AuxDevolucion { Cabecera = existeDevCab, Detalle = existeDevDet, Ext = existingDevExt };
            respuesta.Success = true;
            respuesta.Message = $"La devolución ya había sido registrada previamente con el número: {existeDevCab?.DEV_NUMERO}";
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
                respuesta.Data = respauxdev;
                respuesta.Success = true;
                respuesta.Message = "Devolución guardado exitosamente # de DEVOLUCION = " + respauxdev.Cabecera.DEV_NUMERO;

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
                _logger.LogError("---------------------------------ERROR AL GUARDAR DEVOLUCION: ----------------------" + ex.ToString());
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
        var diasPermitidos = P_DIAS_PERMITIDOS
                         .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(d => d.Trim().ToUpper())
                         .ToList();
        string dayformateado = dayName.ToUpper();
        dayformateado = FormatosTexto.RemoveDiacritics(dayformateado);
        var response = new ServiceResponse<object>();
        try
        {
            var result = await (
                                from rv in _context.REP_VENTAS_INT_PARAM
                                join cl in _context.CLIENTE on rv.CCO_CODCLIPRO equals cl.CLI_CODIGO
                                join cd in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd.CDI_CLIENTE
                                where cl.CLI_AGENTE == agente
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
                                    rv.FAC_CANTIDAD_ORIGINAL,
                                    rv.FAC_CANTIDAD_ORIGINAL2,
                                    rv.EXTEMPORANEO

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

            var fechaLimite = DateTime.Today.AddDays(-365);
            var query1 = from d in _context.TMP_DEV_PROD_CLI_AGE
                         join c in _context.CLIENTE on d.CLIENTE_FK equals c.CLI_CODIGO
                         where d.AGENTE_FK == agente && d.ID_DEVOLUCION_NR != null
                         && d.FECHA_DT.HasValue
                            && d.FECHA_DT.Value >= fechaLimite
                         group d by new { d.ID_DEVOLUCION_NR, d.FECHA_DT, d.NUMERO_IDC, c.CLI_NOMBRE } into g
                         select new
                         {
                             CODIGO = g.Key.ID_DEVOLUCION_NR,
                             NUMERO = g.Key.NUMERO_IDC,
                             FECHA = (DateTime?)g.Key.FECHA_DT,
                             SOURCE = "A",
                             CLIENTE = g.Key.CLI_NOMBRE
                         };

            var query2 = from c in _context.DEVOLUCION_CAB
                         join cl in _context.CLIENTE on c.DEV_CLIENTE equals cl.CLI_CODIGO
                         where c.DEV_AGENTE == agente
                         && c.DEV_FECHA.HasValue
                          && c.DEV_FECHA.Value >= fechaLimite
                         select new
                         {
                             CODIGO = c.DEV_CODIGO,
                             NUMERO = c.DEV_NUMERO,
                             FECHA = c.DEV_FECHA,
                             SOURCE = "N",
                             CLIENTE = cl.CLI_NOMBRE
                         };

            var unionQuery = query1.Union(query2);

            var data = await unionQuery.ToListAsync();

            var resultados = data
                .Select(x => new
                {
                    ZON_NOMBRE = x.CLIENTE + " " + (x.FECHA.HasValue ? x.FECHA.Value.ToString("yyyy-MM-dd") : "") + " - " + x.NUMERO.ToString() + " - ",
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
                         join RV in _context.REP_MOTIVOS_DEV_APPOLD on D.MOTIVO_FK equals RV.TDE_CODIGO
                         where D.AGENTE_FK == dev.AuxDecimal //// trae el agente
                            && (D.ID_DEVOLUCION_NR.ToString() + "A") == dev.AuxString // 
                         select new
                         {
                             PRO_ID = P.PRO_ID,
                             PRO_NOMBRE = P.PRO_NOMBRE,
                             UMD_ID = U.UMD_ID,
                             DMO_CDIGITADA = D.CANTIDAD_NR,
                             MOTIVO_FK = D.MOTIVO_FK,
                             TDE_NOMBRE = RV.TDE_NOMBRE,
                             DVD_SECUENCIA = 0,
                             DVD_OBSERVACION_CALIDAD2 = string.Empty
                         };

            var query2 = from DC in _context.DEVOLUCION_CAB
                         join D in _context.DEVOLUCION_DET on DC.DEV_CODIGO equals D.DVD_CODIGO
                         join P in _context.PRODUCTO on D.DVD_PRODUCTO equals P.PRO_CODIGO
                         join C in _context.CLIENTE on DC.DEV_CLIENTE equals C.CLI_CODIGO
                         join A in _context.AGENTE on DC.DEV_AGENTE equals A.AGE_CODIGO
                         join U in _context.UMEDIDA on D.DVD_UNIDAD equals U.UMD_CODIGO
                         join RV in _context.REP_MOTIVOS_DEV_APPOLD on D.DVD_MOTIVO equals RV.TDE_CODIGO into RVLeft
                         from RV in RVLeft.DefaultIfEmpty()
                         where (D.DVD_CODIGO.ToString() + "N") == Convert.ToString(dev.AuxString)
                         select new
                         {
                             PRO_ID = P.PRO_ID,
                             PRO_NOMBRE = P.PRO_NOMBRE,
                             UMD_ID = U.UMD_ID,
                             DMO_CDIGITADA = D.DVD_CANTIDAD,
                             MOTIVO_FK = D.DVD_MOTIVO,
                             TDE_NOMBRE = RV != null ? RV.TDE_NOMBRE : null,
                             DVD_SECUENCIA = D.DVD_SECUENCIA,
                             DVD_OBSERVACION_CALIDAD2 =
                    // 1. Estado (nvl(dvd_procesa,0))
                    ((D.DVD_PROCESA ?? 0) == 0 ? "No Procesa" : "Si Procesa") +

                    // 2. Obs 1 (concatena solo si no es nulo)
                    (string.IsNullOrEmpty(D.DVD_OBSERVACION_CALIDAD) ? "" : ": " + D.DVD_OBSERVACION_CALIDAD) +

                    // 3. Obs 2 (concatena y convierte a minúsculas)
                    (string.IsNullOrEmpty(D.DVD_OBSERVACION_CALIDAD2) ? "" : " / Cal: " + D.DVD_OBSERVACION_CALIDAD2.ToLower())
                         };

            var resultados = await query1
                .Union(query2)
                .OrderBy(o => o.DVD_SECUENCIA)
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
                         group d by new { d.NUMERO_IDC, a.AGE_NOMBRE, c.CLI_NOMBRE, d.FECHA_DT, d.ESTADO_TX, d.OBSERVACION } into g
                         select new
                         {
                             NUMERO_IDC = g.Key.NUMERO_IDC,
                             CLI_NOMBRE = g.Key.CLI_NOMBRE,
                             FECHA_DT = g.Key.FECHA_DT,
                             ESTADO_TX = g.Key.ESTADO_TX,
                             DEV_OBSERVACION_VALIDADOR = g.Key.OBSERVACION
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
                             ESTADO_TX = d.DEV_ESTADO,
                             DEV_OBSERVACION_VALIDADOR = d.DEV_OBSERVACION_VALIDADOR
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
                        ESTADO = x.ESTADO_TX == "I" ? "INGRESADO " + x.DEV_OBSERVACION_VALIDADOR :
                              x.ESTADO_TX == "G" ? "GRABADO " + x.DEV_OBSERVACION_VALIDADOR :
                              x.ESTADO_TX == "A" ? "ANULADO " + x.DEV_OBSERVACION_VALIDADOR :
                              x.ESTADO_TX == "N" ? "NO PROCESADO " + x.DEV_OBSERVACION_VALIDADOR :
                              x.ESTADO_TX == "P" ? "PROCESADO " + x.DEV_OBSERVACION_VALIDADOR : ""
                    })
             )).ToList();

            unionList.AddRange(
                await Task.WhenAll(list2.Select(async x => new
                {
                    NUMERO_COMPROBANTE = x.NUMERO_IDC != null ? await _carteraService.ObtenerNumeroComprobanteAsync(1, x.NUMERO_IDC ?? 0) : null,
                    NUMERO_IDC = x.NUMERO_IDC ?? 0,
                    CLI_NOMBRE = x.CLI_NOMBRE,
                    FECHA_DT = x.FECHA_DT,
                    ESTADO =
                        (x.ESTADO_TX == "I" ? "INGRESADO " + x.DEV_OBSERVACION_VALIDADOR :
                         x.ESTADO_TX == "G" ? "GRABADO " + x.DEV_OBSERVACION_VALIDADOR :
                         x.ESTADO_TX == "A" ? "ANULADO " + x.DEV_OBSERVACION_VALIDADOR :
                         x.ESTADO_TX == "N" ? "NO PROCESADO " + x.DEV_OBSERVACION_VALIDADOR :
                         x.ESTADO_TX == "P" ? "PROCESADO " + x.DEV_OBSERVACION_VALIDADOR : "")


                         + ((x.NUMERO_IDC != null && x.NUMERO_IDC != 0)
                             ? (" " + await InfoDevoPorcxIDC(x.NUMERO_IDC ?? 0, x.FECHA_DT ?? DateTime.Today))
                             : "")

                }))
            );


            var resultados = unionList.FirstOrDefault();


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
                    TDE_NOMBRE = t.TDE_NOMBRE,
                    DVD_SECUENCIA = d.DMO_SECUENCIA,
                    DVD_OBSERVACION_CALIDAD2 = d.DMO_CONCEPTO
                }
            ).OrderBy(o => o.DVD_SECUENCIA)
            .ToListAsync();


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
                    join dc in _context.DEVOLUCION_CAB on c.CCO_CODIGO equals dc.DEV_DOC_REFERENCIA

                    join dd in _context.DEVOLUCION_DET

                      on new
                      {
                          DevCodigo = (decimal)dc.DEV_CODIGO,
                          ProductCode = (decimal)d.DMO_PRODUCTO
                      }
                       equals new
                       {

                           DevCodigo = (decimal)dd.DVD_CODIGO,


                           ProductCode = dd.DVD_PRODUCTO.GetValueOrDefault()
                       }



                    where c.CCO_CODIGO == dev.AuxDecimal


                    where (dd.DVD_CODIGO == dc.DEV_CODIGO) &&
                          (d.DMO_PRODUCTO == (dd.DVD_PRODUCTO ?? 0)) && // NVL(dd.DVD_PRODUCTO, 0) = d.DMO_PRODUCTO
                          (dd.DVD_MOTIVO == d.DMO_TIPODEV || dd.DVD_MOTIVO_NEG == d.DMO_TIPODEV) // (DD.DVD_MOTIVO = D.DMO_TIPODEV OR DD.DVD_MOTIVO_NEG = D.DMO_TIPODEV)

                    orderby d.DMO_SECUENCIA ascending

                    select new
                    {
                        PRO_ID = p.PRO_ID,
                        PRO_NOMBRE = p.PRO_NOMBRE,
                        UMD_ID = u.UMD_ID,
                        DMO_CDIGITADA = d.DMO_CDIGITADA,
                        TDE_NOMBRE = t.TDE_NOMBRE,
                        DVD_SECUENCIA = d.DMO_SECUENCIA,
                        DVD_OBSERVACION_CALIDAD2 = d.DMO_CONCEPTO
                    }
                ).ToListAsync();

            //         var resultados = await (
            //    from c in _context.CCOMPROBA
            //    join d in _context.DMOVINVI on c.CCO_CODIGO equals d.DMO_CMO_COMPROBA
            //    join p in _context.PRODUCTO on d.DMO_PRODUCTO equals p.PRO_CODIGO
            //    join u in _context.UMEDIDA on d.DMO_UDIGITADA equals u.UMD_CODIGO
            //    join t in _context.TIPODEV on d.DMO_TIPODEV equals t.TDE_CODIGO
            //    join dc in _context.DEVOLUCION_CAB on c.CCO_CODIGO equals dc.DEV_DOC_REFERENCIA


            //    join dd in _context.DEVOLUCION_DET on new
            //    {

            //        DevCodigo = (decimal)dc.DEV_CODIGO,
            //        ProductCode = (decimal)d.DMO_PRODUCTO
            //    }
            //    equals new
            //    {

            //        DevCodigo = (decimal)dd.DVD_CODIGO,
            //        ProductCode = (decimal?)dd.DVD_PRODUCTO ??0 
            //    }

            //    where c.CCO_CODIGO == dev.AuxDecimal
            //    orderby d.DMO_SECUENCIA ascending
            //    select new
            //    {
            //        PRO_ID = p.PRO_ID,
            //        PRO_NOMBRE = p.PRO_NOMBRE,
            //        UMD_ID = u.UMD_ID,
            //        DMO_CDIGITADA = d.DMO_CDIGITADA,
            //        TDE_NOMBRE = t.TDE_NOMBRE,
            //        DVD_OBSERVACION_CALIDAD2 = dd.DVD_OBSERVACION_CALIDAD2
            //    }
            //).ToListAsync();

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




    public async Task<ServiceResponse<object>> GetTopIdvsNoGestxClienteAsync(decimal agente)

    {
        var date = DateTime.Now.AddDays(-P_DIAS_IDVNOGEST); /// inicialmente -60 dias
        var response = new ServiceResponse<object>();
        var clientesDelAgente = await _clientesService.GetCodsClientesDiaxAgente(agente);
        try
        {

            var idvs = await (from ca in _context.DEVOLUCION_CAB
                              where ca.DEV_DOC_REFERENCIA == null && ca.DEV_FECHA >= date
                              && clientesDelAgente.Contains(ca.DEV_CLIENTE ?? 0)
                              select new
                              {
                                  DEV_NUMERO = ca.DEV_NUMERO ?? 0,
                                  DEV_CLIENTE = ca.DEV_CLIENTE ?? 0
                              }).ToListAsync();
            var idvs2 = await (from ca in _context.TMP_DEV_PROD_CLI_AGE
                               where ca.NUMERO_IDC == null && ca.ID_DEVOLUCION_NR != null
                               && ca.FECHA_DT >= date && clientesDelAgente.Contains(ca.CLIENTE_FK)
                               select new
                               {
                                   DEV_NUMERO = ca.ID_DEVOLUCION_NR,
                                   DEV_CLIENTE = ca.CLIENTE_FK
                               }).ToListAsync();


            var todasLasDevoluciones = idvs.Concat(idvs2);


            var resultados = todasLasDevoluciones

                .GroupBy(ca => ca.DEV_CLIENTE)
                .SelectMany(g => g.OrderByDescending(d => d.DEV_NUMERO)
                                   .Take(5))

                .Select(d => new
                {
                    d.DEV_NUMERO,
                    d.DEV_CLIENTE
                })
                .ToList();


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
            _logger.LogError(" --------------------- ERROR ------------------ GetTopIdvsNoGestxClienteAsync() " + ex.ToString());
        }
        return response;


    }



    public async Task<ServiceResponse<object>> GetDevsNoGxAgeAsync(decimal agente)
    {

        var response = new ServiceResponse<object>();
        try
        {

            var fechaLimite = DateTime.Today.AddDays(-365);
            var query1 = from d in _context.TMP_DEV_PROD_CLI_AGE
                         join c in _context.CLIENTE on d.CLIENTE_FK equals c.CLI_CODIGO
                         where d.AGENTE_FK == agente && d.ID_DEVOLUCION_NR != 0
                         && d.FECHA_DT.HasValue
                            && d.FECHA_DT.Value >= fechaLimite

                         group d by new { d.ID_DEVOLUCION_NR, d.FECHA_DT, d.NUMERO_IDC, c.CLI_NOMBRE } into g
                         select new
                         {
                             CODIGO = g.Key.ID_DEVOLUCION_NR,
                             NUMERO = g.Key.NUMERO_IDC,
                             FECHA = (DateTime?)g.Key.FECHA_DT,
                             SOURCE = "A",
                             CLIENTE = g.Key.CLI_NOMBRE
                         };

            var query2 = from c in _context.DEVOLUCION_CAB
                         join cl in _context.CLIENTE on c.DEV_CLIENTE equals cl.CLI_CODIGO
                         where c.DEV_AGENTE == agente
                         && c.DEV_FECHA.HasValue
                          && c.DEV_FECHA.Value >= fechaLimite
                          && c.DEV_DOC_REFERENCIA == null
                         select new
                         {
                             CODIGO = c.DEV_CODIGO,
                             NUMERO = c.DEV_NUMERO,
                             FECHA = c.DEV_FECHA,
                             SOURCE = "N",
                             CLIENTE = cl.CLI_NOMBRE
                         };

            var unionQuery = query1.Union(query2);

            var data = await unionQuery.ToListAsync();

            var resultados = data
                .Select(x => new
                {
                    ZON_NOMBRE = x.CLIENTE + " " + (x.FECHA.HasValue ? x.FECHA.Value.ToString("yyyy-MM-dd") : "") + " - " + x.NUMERO.ToString() + " - ",
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



    public async Task<string> InfoDevoPorcxIDC(decimal idc, DateTime fechaIdv)
    {
        try
        {
            var idcsData = await _context.CCOMPROBA
            .Where(c => c.CCO_CODIGO == idc)
            .Select(r => new { r.CCO_ALMACEN })
            .ToListAsync();

            var almacen = idcsData.FirstOrDefault()?.CCO_ALMACEN ?? 0;

            var diasRestars = await _context.CPARAMET ///////////ojo se toma de la tabla de parametros  CPARAMET
                .Where(d => d.CPA_SECUENCIA == "DEV"
                         && d.CPA_SIGLA == "CXC"
                         && d.CPA_EMPRESA == p_empresa)
                .Select(d => d.CPA_VALOR)
                .ToListAsync();

            var diasRestar = diasRestars.FirstOrDefault();
            var valorParametros = await (from c in _context.CPARAMET
                                         join p in _context.DPARAMET
                                              on new { Cod = c.CPA_CODIGO, Emp = c.CPA_EMPRESA }
                                              equals new { Cod = p.DPA_CPA_CODIGO, Emp = p.DPA_EMPRESA }
                                         where c.CPA_SECUENCIA == "DEP"
                                            && c.CPA_SIGLA == "CXC"
                                            && c.CPA_EMPRESA == p_empresa
                                            && p.DPA_ALMACEN == almacen
                                         select p.DPA_VALOR)
                                        .ToListAsync();
            var valorParametro = valorParametros.FirstOrDefault();
            DateTime fechaDesde = fechaIdv.AddDays(-(double)(diasRestar ?? 0));
            decimal porcentajeMaximo = 100 - (valorParametro ?? 0);
            string mensaje = $"Facturas desde {fechaDesde:dd/MM/yyyy} hasta {fechaIdv:dd/MM/yyyy}, devolucion maxima del {porcentajeMaximo:0}%.";

            return mensaje;
        }
        catch (Exception ex)
        {
            _logger.LogError(" --------------------- ERROR ------------------ InfoDevoPorcxIDC() " + ex.ToString() + idc);
            return "Error al obtener la información de devolución.";
        }
    }




    public async Task<ServiceResponse<VM_REPORTE_VENTAS2023>> GetMetaDevs(decimal agenteId)
    {

        DateTime hoy = DateTime.Today; // O DateTime.Now

        int mes = hoy.Month;
        int anio = hoy.Year;
        var clasificacionesExcluidas = new[] { "NAVIDEÑO", "NA" };
        var response = new ServiceResponse<VM_REPORTE_VENTAS2023>();

        try
        {

            var queryVentas = from vl in _context.VM_REPORTE_VENTAS2023
                              join a in _context.AGENTE on vl.CCO_AGENTE equals a.AGE_CODIGO
                              where vl.CCO_AGENTE == agenteId
                                 && vl.MESES == mes
                                 && vl.ANIOS == anio
                                 && !clasificacionesExcluidas.Contains(vl.CLASIFICACION)
                              group vl by a.AGE_ALMACEN into g
                              select new
                              {
                                  AlmacenId = g.Key,

                                  TotalFacs = g.Sum(x => (x.TOTAL_LIBRAS ?? 0) > 0 ? (x.TOTAL_LIBRAS ?? 0) : 0),

                                  TotalNcc = g.Sum(x => (x.TOTAL_LIBRAS ?? 0) < 0 ? (x.TOTAL_LIBRAS ?? 0) : 0)
                              };


            var listaVentas = await queryVentas.ToListAsync();


            var ventasData = listaVentas.FirstOrDefault();

            // Si no hay datos, retornamos ceros
            if (ventasData == null)
            {
                response.Data = new VM_REPORTE_VENTAS2023 { TOTAL_FACS = 0m, TOTAL_NCC = 0m, PORCENTAJE_DEVS = 0m, METAPORCDEV = 0m, RANKING = 0 };
                response.Success = true;
                response.Message = "NO SE ENCOTRARON DATOS DE DEVOLUCIONES";
                return response;
            }

            decimal totalFacs = ventasData.TotalFacs;
            decimal totalNcc = Math.Abs(ventasData.TotalNcc); // Convertimos a positivo aquí

            var queryParam = from c in _context.CPARAMET
                             join p in _context.DPARAMET
                                  on new { Cod = c.CPA_CODIGO, Emp = c.CPA_EMPRESA }
                                  equals new { Cod = p.DPA_CPA_CODIGO, Emp = p.DPA_EMPRESA }
                             where c.CPA_SECUENCIA == "DEP"
                                && c.CPA_SIGLA == "CXC"
                                && c.CPA_EMPRESA == 1
                                && p.DPA_ALMACEN == ventasData.AlmacenId
                             select p.DPA_VALOR;


            var listaParam = await queryParam.ToListAsync();
            var valorParametro = listaParam.FirstOrDefault();

            decimal porcDevParam = 100 - (valorParametro ?? 0);


            decimal porcentajeDevs = 0;

            if (totalFacs > 0)
            {
                // Fórmula: (NCC / FACS) * 100
                porcentajeDevs = Math.Round((totalNcc / totalFacs) * 100, 2);
            }


            decimal metaMaxima = porcDevParam > 0 ? porcDevParam : 100;
            decimal tercio = metaMaxima / 3;

            int ranking;

            if (porcentajeDevs <= tercio)
            {
                ranking = 1; // Primer tercio (Mejor)
            }
            else if (porcentajeDevs <= (tercio * 2))
            {
                ranking = 2; // Segundo tercio (Medio)
            }
            else
            {
                ranking = 3; // Tercer tercio o superior (Peor)
            }

            response.Data = new VM_REPORTE_VENTAS2023
            {
                TOTAL_FACS = totalFacs,
                TOTAL_NCC = totalNcc,
                PORCENTAJE_DEVS = porcentajeDevs,
                METAPORCDEV = porcDevParam,
                RANKING = ranking
            };
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXITOSAMENTE";
            return response;
        }
        catch (Exception ex)
        {
            // Manejo de error o log
            _logger.LogError(ex, "Error al calcular reporte de devoluciones");
            response.Data = new VM_REPORTE_VENTAS2023
            {
                TOTAL_FACS = 0m,
                TOTAL_NCC = 0m,
                PORCENTAJE_DEVS = 0m,
                METAPORCDEV = 0m,
                RANKING = 0
            };
            response.Success = true;
            response.Message = "DATOS ENCONTRADOS EXITOSAMENTE";
            return response;
        }
    }
}
