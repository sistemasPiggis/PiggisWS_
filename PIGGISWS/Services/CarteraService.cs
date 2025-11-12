using Azure;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Models.Vistas;
using PIGGISWS.Services.Utils;
using System.Data;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace PIGGISWS.Services;

public class CarteraService : ICarteraService
{

    private readonly ApplicationDbContext _context;
    private readonly IAgenteService _agenteService;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();
    private readonly ILogger<PedidoService> _logger;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;
    int p_car_siglafac;
    decimal CRT_NUMERO;
    int p_dias_anticipo;
    string usuarioAg;
    string P_LISTA_PNAV = "";

    public CarteraService(ApplicationDbContext context, IAgenteService agenteService, ILogger<PedidoService> logger)
    {
        _context = context;
        GetParametros();
        _agenteService = agenteService;
        usuarioAg = string.Empty;
        _logger = logger;
    }
    public void GetParametros()
    {
        try
        {
            parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "CarteraService" || p.SERVICIO == "GENERAL").ToList();
            p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
            p_car_siglafac = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 55)?.VALOR ?? "0");
            p_dias_anticipo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 57)?.VALOR ?? "0");
            P_LISTA_PNAV = parametros.First(p => p.CODIGO == 61)?.VALOR ?? "";

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            _logger.LogError(" --------------------- ERROR ------------------ GetParametros() " + ex.ToString());
        }

    }

    public async Task<ServiceResponse<object>> GetCarteraxFacturaAsync(Cartera cartera)
    {
        var response = new ServiceResponse<object>();
        try
        {
            var saldo = await (from c in _context.CCOMPROBA
                               join dd in _context.DDOCUMENTO on c.CCO_CODIGO equals dd.DDO_CCO_COMPROBA
                               join a in _context.AGENTE on c.CCO_AGENTE equals a.AGE_CODIGO
                               join cl in _context.CLIENTE on c.CCO_CODCLIPRO equals cl.CLI_CODIGO
                               join zo in _context.ZONA on cl.CLI_ZONA equals zo.ZON_CODIGO
                               where dd.DDO_AGENTE == cartera.CRT_AGENTE
                               && (dd.DDO_MONTO - (dd.DDO_CANCELA ?? 0)) > 0
                               && dd.DDO_DOCTRAN.Contains(cartera.CRT_DOCTRAN ?? "")
                               select new AuxCartera
                               {
                                   dDocumento = dd,
                                   SALDO = dd.DDO_MONTO - (dd.DDO_CANCELA ?? 0),
                                   CLI_NOMBRE = cl.CLI_NOMBRE,
                                   ZON_NOMBRE = zo.ZON_NOMBRE,
                                   AGE_NOMBRE = a.AGE_NOMBRE
                               }
                               ).ToListAsync();


            if (saldo == null || !saldo.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS PARA LOS DOCUMENTOS SELECCIONADOS";
                return response;
            }

            response.Data = saldo;
            response.Success = true;
            response.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetCarteraxFacturaAsync() " + ex.ToString() + cartera);
        }
        return response;
    }



    public async Task<ServiceResponse<object>> GetCarteraxFacturaDiaAsync(Cartera cartera)
    {
        System.DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

        // Crea un objeto CultureInfo en español
        CultureInfo ci = new CultureInfo("es-ES");

        // Obtiene el nombre del día de la semana en español
        string dayName = ci.DateTimeFormat.GetDayName(dayOfWeek);

        string dayformateado = dayName.ToUpper();
        dayformateado = FormatosTexto.RemoveDiacritics(dayformateado);
        var response = new ServiceResponse<object>();
        try
        {
            var saldo = await (from
                               dd in _context.DDOCUMENTO
                               join a in _context.AGENTE on dd.DDO_AGENTE equals a.AGE_CODIGO
                               join cl in _context.CLIENTE on dd.DDO_CODCLIPRO equals cl.CLI_CODIGO
                               join zo in _context.ZONA on cl.CLI_ZONA equals zo.ZON_CODIGO
                               join cd in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd.CDI_CLIENTE
                               //join ca in _context.CARTERA on dd.DDO_CCO_COMPROBA equals ca.CRT_DDO_COMPROBA
                               where dd.DDO_AGENTE == cartera.CRT_AGENTE
                               && (dd.DDO_MONTO - (dd.DDO_CANCELA ?? 0)) > 0
                               //&& dd.DDO_DOCTRAN.Contains(cartera.CRT_DOCTRAN)
                               && cd.CDI_DIA == dayformateado
                               && dd.DDO_DEBCRE == 1
                               select new AuxCartera
                               {
                                   dDocumento = dd,
                                   //cartera = ca,
                                   SALDO = dd.DDO_MONTO - (dd.DDO_CANCELA ?? 0),
                                   CLI_NOMBRE = cl.CLI_NOMBRE,
                                   ZON_NOMBRE = zo.ZON_NOMBRE,
                                   CRT_MONTO = dd.DDO_MONTO
                                   ///AGE_NOMBRE = a.AGE_NOMBRE
                               }
                               ).OrderBy(c => c.dDocumento.DDO_FECHA_VEN)
                                .ToListAsync();


            if (saldo == null || !saldo.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS PARA LOS DOCUMENTOS SELECCIONADOS";
                return response;
            }

            response.Data = saldo;
            response.Success = true;
            response.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetCarteraxFacturaDiaAsync() " + ex.ToString() + cartera);
        }
        return response;
    }


    public async Task<ServiceResponse<object>> GetCarteraxFacDiaCliAsync(Cartera cartera, decimal cliente) /// trae las FACTURAS de  los clientes del día de gestión
    {
        System.DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

        // Crea un objeto CultureInfo en español
        CultureInfo ci = new CultureInfo("es-ES");

        // Obtiene el nombre del día de la semana en español
        string dayName = ci.DateTimeFormat.GetDayName(dayOfWeek);

        string dayformateado = dayName.ToUpper();
        dayformateado = FormatosTexto.RemoveDiacritics(dayformateado);
        var response = new ServiceResponse<object>();
        try
        {
            var saldo = await (from
                               dd in _context.DDOCUMENTO
                               join a in _context.AGENTE on dd.DDO_AGENTE equals a.AGE_CODIGO
                               join cl in _context.CLIENTE on dd.DDO_CODCLIPRO equals cl.CLI_CODIGO
                               join zo in _context.ZONA on cl.CLI_ZONA equals zo.ZON_CODIGO
                               join cd in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd.CDI_CLIENTE
                               //join ca in _context.CARTERA on dd.DDO_CCO_COMPROBA equals ca.CRT_DDO_COMPROBA
                               where dd.DDO_AGENTE == cartera.CRT_AGENTE
                               && (dd.DDO_MONTO - (dd.DDO_CANCELA ?? 0)) > 0
                               //&& dd.DDO_DOCTRAN.Contains(cartera.CRT_DOCTRAN)
                               && cd.CDI_DIA == dayformateado
                               && dd.DDO_DEBCRE == 1
                               && cl.CLI_CODIGO == cliente
                               select new AuxCartera
                               {
                                   dDocumento = dd,
                                   //cartera = ca,
                                   SALDO = dd.DDO_MONTO - (dd.DDO_CANCELA ?? 0),
                                   CLI_NOMBRE = cl.CLI_NOMBRE,
                                   ZON_NOMBRE = zo.ZON_NOMBRE,
                                   ///AGE_NOMBRE = a.AGE_NOMBRE
                               }
                               ).OrderBy(c => c.CRT_FECHA).ToListAsync();


            if (saldo == null || !saldo.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DATOS PARA LOS DOCUMENTOS SELECCIONADOS";
                return response;
            }

            response.Data = saldo;
            response.Success = true;
            response.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetCarteraxFacDiaCliAsync() " + ex.ToString() + cartera + cliente);
        }
        return response;
    }



    public async Task<ServiceResponse<object>> GetCarteraxAgenteReporteAsync(int agente)
    {
        var response = new ServiceResponse<object>();
        try
        {
            var _reporte = await (from c in _context.CARTERA
                                  join a in _context.AGENTE on c.CRT_AGENTE equals a.AGE_CODIGO
                                  join cl in _context.CLIENTE on c.CRT_CLIENTE equals cl.CLI_CODIGO
                                  join z in _context.ZONA on cl.CLI_ZONA equals z.ZON_CODIGO
                                  join d in _context.DDOCUMENTO on c.CRT_DDO_COMPROBA equals d.DDO_CCO_COMPROBA
                                  where (c.CRT_ESTADO ?? 0) == 1
                                        && (c.CRT_PROCESADA ?? 0) == 0
                                        && c.CRT_EMPLEADO == null
                                        && c.CRT_AGENTE == agente
                                  orderby c.CRT_SECUENCIA ascending
                                  select new
                                  {
                                      CRT_NUMERO = c.CRT_NUMERO,
                                      ZON_NOMBRE = z.ZON_NOMBRE,
                                      CLI_NOMBRE = cl.CLI_NOMBRE,
                                      c.CRT_DOCTRAN,
                                      c.CRT_MONTO,
                                      SALDO = (d.DDO_MONTO) - (d.DDO_CANCELA ?? 0),
                                      CRT_CANCELA = (c.CRT_CANCELA ?? 0),
                                      CRT_CANCELA_CH = (c.CRT_CANCELA_CH ?? 0),
                                      AGE_NOMBRE = a.AGE_NOMBRE,
                                      CRT_FECHA = c.CRT_FECHA
                                  }).ToListAsync();



            if (_reporte == null || !_reporte.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "SE CREARÁ UN NUEVO REPORTE";
                return response;
            }

            response.Data = _reporte;
            response.Success = true;
            response.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetCarteraxAgenteReporteAsync() " + ex.ToString() + agente);
        }
        return response;
    }




    public async Task<ServiceResponse<object>> CreateFacturaCarteraAsync(AuxCartera cartera)
    {
        var response = new ServiceResponse<object>();
        try
        {
            decimal agente = cartera.cartera?.CRT_AGENTE ?? 0;
            usuarioAg = await _agenteService.GetUsuarioAsync(agente);

            if (cartera != null && cartera.cartera != null && cartera.dDocumento != null)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    var facturasr = await _context.CARTERA
                        .Where(r => r.CRT_NUMERO == cartera.CRT_NUMERO)
                        .ToListAsync();
                    var ultimafactura = facturasr
                        .OrderByDescending(r => r.CRT_SECUENCIA)
                        .FirstOrDefault();


                    var reporte = await GetCarteraxAgenteReporteAsync(cartera.cartera.CRT_AGENTE ?? 0);

                    if (reporte.Data == null && reporte.Success == true)
                    {



                        using (var command = _context.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = "SELECT CARTERA_S_CRT_NUMERO.NEXTVAL FROM dual";
                            await _context.Database.OpenConnectionAsync();
                            var result = await command.ExecuteScalarAsync();
                            CRT_NUMERO = Convert.ToDecimal(result);
                        }

                        if (cartera.cartera != null && cartera.dDocumento != null)
                        {
                            var _cartera = new Cartera
                            {
                                CRT_EMPRESA = cartera.dDocumento.DDO_EMPRESA,
                                CRT_DDO_COMPROBA = cartera.dDocumento.DDO_CCO_COMPROBA,
                                CRT_TRANSACC = 1, ////// realizar select
                                CRT_DOCTRAN = cartera.dDocumento.DDO_DOCTRAN,
                                CRT_PAGO = cartera.cartera.CRT_PAGO,
                                CRT_FECHA = cartera.cartera.CRT_FECHA,
                                CRT_CLIENTE = cartera.dDocumento.DDO_CODCLIPRO,
                                CRT_AGENTE = cartera.cartera.CRT_AGENTE,
                                CRT_MONTO = cartera.cartera.CRT_MONTO,
                                CRT_EMPLEADO = cartera.cartera.CRT_EMPLEADO,
                                CRT_NUMERO = CRT_NUMERO,
                                CRT_PROCESADA = null,
                                CRT_CANCELA = cartera.cartera.CRT_CANCELA,
                                CRT_CANCELA_CH = cartera.cartera.CRT_CANCELA_CH ?? 0,
                                CRT_SECUENCIA = 1,
                                CRT_ESTADO = 1,
                                CREA_USR = usuarioAg
                            };

                            await _context.CARTERA.AddAsync(_cartera);
                            int carterasave = await _context.SaveChangesAsync();

                            if (carterasave > 0)
                            {


                                var hoy = DateTime.Today;
                                var ruteros = await _context.RUTERO
                                    .Where(r => r.RUT_CLIENTE == _cartera.CRT_CLIENTE && r.RUT_FECHA == hoy.Date
                                    && r.RUT_AGENTE == _cartera.CRT_AGENTE).ToListAsync();

                                var rutero = ruteros.FirstOrDefault();
                                if (rutero != null)
                                {
                                    rutero.RUT_COBRO = 1;
                                    rutero.RUT_VISITA = 1;
                                    await _context.SaveChangesAsync();
                                    await transaction.CommitAsync();
                                }
                                else
                                {
                                    var zonas = await _context.CLIENTE.Where(c => c.CLI_CODIGO == _cartera.CRT_CLIENTE)
                                                .Select(c => c.CLI_ZONA).ToListAsync();
                                    var zona = zonas.FirstOrDefault();
                                    var nuevoRutero = new Rutero
                                    {
                                        RUT_CLIENTE = _cartera.CRT_CLIENTE ?? 0,
                                        RUT_FECHA = hoy.Date,
                                        RUT_COBRO = 1,
                                        RUT_VISITA = 1,
                                        RUT_EMPRESA = p_empresa,
                                        RUT_ZONA = zona ?? 0,
                                        RUT_AGENTE = _cartera.CRT_AGENTE ?? 0

                                    };

                                    await _context.RUTERO.AddAsync(nuevoRutero);
                                    await _context.SaveChangesAsync();
                                    await transaction.CommitAsync();
                                }




                                await DesbloquearClienteAsync(usuarioAg, _cartera.CRT_CLIENTE ?? 0);


                                response.Data = _cartera;
                                response.Success = true;
                                response.Message = "EL REGISTRO SE GUARDO EXITOSAMENTE";
                                return response;
                            }
                        }
                    }

                    else
                    {

                        if (ultimafactura != null) // si existe el reporte 
                        {
                            int sec = ultimafactura.CRT_SECUENCIA ?? 1;
                            decimal tran = ultimafactura.CRT_TRANSACC ?? 0;
                            var factura = await _context.CARTERA
                                .Where(c => c.CRT_NUMERO == cartera.CRT_NUMERO && c.CRT_DOCTRAN == cartera.dDocumento.DDO_DOCTRAN)
                                .ToListAsync(); /// trae la factura repetida
                            var _factura = factura.FirstOrDefault();

                            if (_factura != null)
                            {
                                response.Data = null;
                                response.Success = true;
                                response.Message = "EL DOCUMENTO YA ESTA REGISTRADO EN ESTE REPORTE POR FAVOR COMUNICARSE CON EL DEPARTAMENTO DE CARTERA.";
                                return response;
                            }
                            else
                            {
                                if (cartera.cartera != null && cartera.dDocumento != null)
                                {
                                    var _cartera = new Cartera
                                    {
                                        CRT_EMPRESA = cartera.dDocumento.DDO_EMPRESA,
                                        CRT_DDO_COMPROBA = cartera.dDocumento.DDO_CCO_COMPROBA,
                                        CRT_TRANSACC = tran,   /// obtener de consulta puede ser 1
                                        CRT_DOCTRAN = cartera.dDocumento.DDO_DOCTRAN,
                                        CRT_PAGO = cartera.cartera.CRT_PAGO,
                                        CRT_FECHA = cartera.cartera.CRT_FECHA,
                                        CRT_CLIENTE = cartera.dDocumento.DDO_CODCLIPRO,
                                        CRT_AGENTE = cartera.cartera.CRT_AGENTE,
                                        CRT_MONTO = cartera.dDocumento.DDO_MONTO,
                                        CRT_EMPLEADO = cartera.cartera.CRT_EMPLEADO,
                                        CRT_NUMERO = cartera.CRT_NUMERO ?? 0,
                                        CRT_PROCESADA = null,
                                        CRT_CANCELA = cartera.cartera.CRT_CANCELA,
                                        CRT_CANCELA_CH = cartera.cartera.CRT_CANCELA_CH ?? 0,
                                        CRT_SECUENCIA = sec + 1,
                                        CRT_ESTADO = 1,
                                        CREA_USR = usuarioAg
                                    };

                                    await _context.CARTERA.AddAsync(_cartera);
                                    int carterasave = await _context.SaveChangesAsync();

                                    if (carterasave > 0)
                                    {

                                        var hoy = DateTime.Today;
                                        var ruteros = await _context.RUTERO
                                            .Where(r => r.RUT_CLIENTE == _cartera.CRT_CLIENTE && r.RUT_FECHA == hoy.Date && r.RUT_AGENTE == _cartera.CRT_AGENTE).ToListAsync();
                                        var rutero = ruteros.FirstOrDefault();

                                        if (rutero != null)
                                        {
                                            if (usuarioAg != null)
                                            {
                                                _cartera.CREA_USR = usuarioAg;
                                                _context.CARTERA.Update(_cartera);
                                                await _context.SaveChangesAsync();

                                            }
                                            rutero.RUT_COBRO = 1;
                                            rutero.RUT_VISITA = 1;
                                            await _context.SaveChangesAsync();
                                            await transaction.CommitAsync();
                                        }
                                        else
                                        {
                                            var zonas = await _context.CLIENTE.Where(c => c.CLI_CODIGO == _cartera.CRT_CLIENTE)
                                                             .Select(c => c.CLI_ZONA).ToListAsync();
                                            var zona = zonas.FirstOrDefault();
                                            var nuevoRutero = new Rutero
                                            {
                                                RUT_CLIENTE = _cartera.CRT_CLIENTE ?? 0,
                                                RUT_FECHA = hoy.Date,
                                                RUT_COBRO = 1,
                                                RUT_VISITA = 1,
                                                RUT_EMPRESA = p_empresa,
                                                RUT_ZONA = zona ?? 0,
                                                RUT_AGENTE = _cartera.CRT_AGENTE ?? 0
                                            };
                                            if (usuarioAg != null)
                                            {
                                                _cartera.CREA_USR = usuarioAg;
                                                _context.CARTERA.Update(_cartera);
                                                await _context.SaveChangesAsync();

                                            }

                                            await _context.RUTERO.AddAsync(nuevoRutero);
                                            await _context.SaveChangesAsync();
                                            await transaction.CommitAsync();
                                        }
                                        await DesbloquearClienteAsync(usuarioAg, _cartera.CRT_CLIENTE ?? 0);

                                        response.Data = _cartera;
                                        response.Success = true;
                                        response.Message = "EL REGISTRO SE GUARDO EXITOSAMENTE";
                                        return response;
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        response.Data = null;
                                        response.Success = false;
                                        response.Message = "Existió un problema por favor vuelva a intentarlo.";
                                        return response;
                                    }
                                }
                                else
                                {
                                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                                    {
                                        command.CommandText = "SELECT CARTERA_S_CRT_NUMERO.NEXTVAL FROM dual;";
                                        await _context.Database.OpenConnectionAsync();
                                        var result = await command.ExecuteScalarAsync();
                                        CRT_NUMERO = Convert.ToInt16(result);
                                    }

                                    if (cartera.cartera != null && cartera.dDocumento != null)
                                    {
                                        var _cartera = new Cartera
                                        {
                                            CRT_EMPRESA = cartera.dDocumento.DDO_EMPRESA,
                                            CRT_DDO_COMPROBA = cartera.dDocumento.DDO_CCO_COMPROBA,
                                            CRT_TRANSACC = 1, ////// realizar select
                                            CRT_DOCTRAN = cartera.dDocumento.DDO_DOCTRAN,
                                            CRT_PAGO = cartera.cartera.CRT_PAGO,
                                            CRT_FECHA = cartera.cartera.CRT_FECHA,
                                            CRT_CLIENTE = cartera.dDocumento.DDO_CODCLIPRO,
                                            CRT_AGENTE = cartera.cartera.CRT_AGENTE,
                                            CRT_MONTO = cartera.cartera.CRT_MONTO,
                                            CRT_EMPLEADO = cartera.cartera.CRT_EMPLEADO,
                                            CRT_NUMERO = CRT_NUMERO,
                                            CRT_PROCESADA = null,
                                            CRT_CANCELA = cartera.cartera.CRT_CANCELA,
                                            CRT_CANCELA_CH = cartera.CRT_CANCELA_CH,
                                            CRT_SECUENCIA = 1,
                                            CRT_ESTADO = 1,
                                            CREA_USR = usuarioAg
                                        };

                                        await _context.CARTERA.AddAsync(_cartera);
                                        int carterasave = await _context.SaveChangesAsync();

                                        if (carterasave > 0)
                                        {
                                            var hoy = DateTime.Today;
                                            var ruteros = await _context.RUTERO
                                                .Where(r => r.RUT_CLIENTE == _cartera.CRT_CLIENTE && r.RUT_FECHA == hoy.Date && r.RUT_AGENTE == _cartera.CRT_AGENTE).ToListAsync();

                                            var rutero = ruteros.FirstOrDefault();
                                            if (rutero != null)
                                            {
                                                if (usuarioAg != null)
                                                {
                                                    _cartera.CREA_USR = usuarioAg;
                                                    _context.CARTERA.Update(_cartera);
                                                    await _context.SaveChangesAsync();

                                                }

                                                rutero.RUT_COBRO = 1;
                                                rutero.RUT_VISITA = 1;
                                                await _context.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                var zonas = await _context.CLIENTE.Where(c => c.CLI_CODIGO == _cartera.CRT_CLIENTE)
                                                            .Select(c => c.CLI_ZONA).ToListAsync();
                                                var zona = zonas.FirstOrDefault();
                                                var nuevoRutero = new Rutero
                                                {
                                                    RUT_CLIENTE = _cartera.CRT_CLIENTE ?? 0,
                                                    RUT_FECHA = hoy.Date,
                                                    RUT_COBRO = 1,
                                                    RUT_VISITA = 1,
                                                    RUT_EMPRESA = p_empresa,
                                                    RUT_ZONA = zona ?? 0,
                                                    RUT_AGENTE = _cartera.CRT_AGENTE ?? 0

                                                };
                                                if (usuarioAg != null)
                                                {
                                                    _cartera.CREA_USR = usuarioAg;
                                                    _context.CARTERA.Update(_cartera);
                                                    await _context.SaveChangesAsync();

                                                }
                                                await _context.RUTERO.AddAsync(nuevoRutero);
                                                await _context.SaveChangesAsync();
                                                await transaction.CommitAsync();
                                            }

                                            await DesbloquearClienteAsync(usuarioAg, _cartera.CRT_CLIENTE ?? 0);

                                            response.Data = _cartera;
                                            response.Success = true;
                                            response.Message = "EL REGISTRO SE GUARDO EXITOSAMENTE";
                                            return response;
                                        }
                                    }
                                }
                            }
                        }

                        response.Data = null;
                        response.Success = false;
                        response.Message = "No se encontró el reporte.";
                        return response;
                    }
                }
            }

            response.Data = null;
            response.Success = false;
            response.Message = "Los datos proporcionados son nulos.";
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ CreateFacturaCarteraAsync() " + ex.ToString() + cartera);
            return response;
        }
    }


    public async Task<ServiceResponse<object>> GetCarteraXClienteAsync(decimal Cliente)
    {
        var response = new ServiceResponse<object>();
        try
        {
            var deudas = await (from
                               dd in _context.REP_CONS_CARTERA_INTERNETA

                                select new Rep_Cons_Cartera_Interneta
                                {
                                    DOC = dd.DOC,
                                    DDO_DEBCRE = dd.DDO_DEBCRE,
                                    DDO_CODCLIPRO = dd.DDO_CODCLIPRO,
                                    CLI_NOMBRE = dd.CLI_NOMBRE,
                                    DDO_MONTO = dd.DDO_MONTO,
                                    DDO_FECHA_VEN = dd.DDO_FECHA_VEN,
                                    DDO_FECHA_EMI = dd.DDO_FECHA_EMI,
                                    CANCELA = dd.CANCELA,
                                    SALDO = dd.SALDO,
                                    SALDOT = dd.SALDOT,
                                    CCO_TIPODOC = dd.CCO_TIPODOC,
                                    CCO_SIGLA = dd.CCO_SIGLA

                                }
                               ).Where(c => c.DDO_CODCLIPRO == Cliente && c.DDO_DEBCRE == 1)
                               .OrderBy(c => c.DDO_FECHA_EMI)
                               .ToListAsync();



            if (deudas == null || !deudas.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DEUDAS PARA EL CLIENTE SELECCIONADO";
                return response;
            }

            response.Data = deudas;
            response.Success = true;
            response.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ CreateFacturaCarteraAsync() " + ex.ToString() + Cliente);
        }
        return response;
    }


    public async Task<ServiceResponse<object>> GetAnticiposXClienteAsync(decimal Cliente)
    {
        var response = new ServiceResponse<object>();
        DateTime fechalimite = DateTime.Now.AddDays(-p_dias_anticipo);
        try
        {
            var anticipos = await (from
                               dd in _context.REP_CONS_CARTERA_INTERNETA

                                   select new Rep_Cons_Cartera_Interneta
                                   {
                                       DOC = dd.DOC,
                                       DDO_DEBCRE = dd.DDO_DEBCRE,
                                       DDO_CODCLIPRO = dd.DDO_CODCLIPRO,
                                       CLI_NOMBRE = dd.CLI_NOMBRE,
                                       DDO_MONTO = dd.DDO_MONTO,
                                       DDO_FECHA_VEN = dd.DDO_FECHA_VEN,
                                       DDO_FECHA_EMI = dd.DDO_FECHA_EMI,
                                       CANCELA = dd.CANCELA,
                                       SALDO = dd.SALDO,
                                       SALDOT = dd.SALDOT,
                                       CCO_TIPODOC = dd.CCO_TIPODOC,
                                       CCO_SIGLA = dd.CCO_SIGLA

                                   }
                               ).Where(c => c.DDO_CODCLIPRO == Cliente && c.DDO_DEBCRE == 2 &&
                               c.DDO_FECHA_EMI > fechalimite)
                               .OrderBy(c => c.DDO_FECHA_EMI)
                               .ToListAsync();



            if (anticipos == null || !anticipos.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA ANTICIPOS PARA EL CLIENTE SELECCIONADO";
                return response;
            }

            response.Data = anticipos;
            response.Success = true;
            response.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetAnticiposXClienteAsync() " + ex.ToString() + Cliente);
        }
        return response;
    }

    public async Task DesbloquearClienteAsync(string usuario, decimal clienteCodigo)
    {
        try
        {
            // Crear los parámetros para el procedimiento almacenado
            var paramClienteCodigo = new OracleParameter("P_CLI_CODIGO", OracleDbType.Decimal)
            {
                Value = clienteCodigo,
                Direction = ParameterDirection.Input
            };

            var paramUsuario = new OracleParameter("P_USUARIO", OracleDbType.Varchar2)
            {
                Value = usuario,
                Direction = ParameterDirection.Input
            };

            // Ejecutar el procedimiento almacenado
            await _context.Database.ExecuteSqlRawAsync(
                "BEGIN P_CXC_DESBLOQUEO_XCLIENTE(:P_CLI_CODIGO, :P_USUARIO); END;",
                paramClienteCodigo,
                paramUsuario
            );

            Console.WriteLine("Procedimiento ejecutado exitosamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al ejecutar el procedimiento almacenado: {ex.Message}");
            _logger.LogError(" --------------------- ERROR ------------------ DesbloquearClienteAsync() " + ex.ToString() + usuario + clienteCodigo);
            throw;
        }
    }


    public async Task<ServiceResponse<object>> GetReportesxAgenteAsync(decimal agente)
    {
        var response = new ServiceResponse<object>();
        try
        {
            var yearNow = DateTime.Now.Year;
            var resultados = await _context.CARTERA
                .Where(ct => ct.CRT_AGENTE == agente
                    && ((ct.CRT_CANCELA ?? 0) != 0 || (ct.CRT_CANCELA_CH ?? 0) != 0)
                    && ct.CRT_FECHA.HasValue && ct.CRT_FECHA.Value.Year == yearNow)
                .Select(ct => new
                {
                    CRT_DOCTRAN = (ct.CRT_FECHA.HasValue ? ct.CRT_FECHA.Value.ToString("yyyy-MM-dd") : "") + "-" + ct.CRT_NUMERO,
                    CRT_NUMERO = ct.CRT_NUMERO,
                    CRT_AGENTE = ct.CRT_AGENTE
                })
                .Distinct()
                .OrderByDescending(x => x.CRT_NUMERO)
                .ToListAsync();


            if (resultados == null || !resultados.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA REPORTES";
                return response;
            }

            response.Data = resultados;
            response.Success = true;
            response.Message = "REPORTES ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetReportesxAgenteAsync() " + ex.ToString() + agente);
        }
        return response;
    }

    public async Task<ServiceResponse<object>> GetReportexNumeroAsync(Cartera cartera)
    {
        var response = new ServiceResponse<object>();
        try
        {
            var resultados = await (
                from ca in _context.CARTERA
                join cl in _context.CLIENTE on ca.CRT_CLIENTE equals cl.CLI_CODIGO
                join z in _context.ZONA on cl.CLI_ZONA equals z.ZON_CODIGO
                join a in _context.AGENTE on ca.CRT_AGENTE equals a.AGE_CODIGO
                join d in _context.DDOCUMENTO on new { ca.CRT_DDO_COMPROBA, ca.CRT_TRANSACC, ca.CRT_DOCTRAN, ca.CRT_PAGO }
                                              equals new
                                              {
                                                  CRT_DDO_COMPROBA = d.DDO_CCO_COMPROBA,
                                                  CRT_TRANSACC = d.DDO_TRANSACC,
                                                  CRT_DOCTRAN = d.DDO_DOCTRAN,
                                                  CRT_PAGO = d.DDO_PAGO
                                              }
                where ca.CRT_NUMERO == cartera.CRT_NUMERO
                select new
                {
                    ZONA = z.ZON_NOMBRE,
                    AGE_NOMBRE = a.AGE_NOMBRE,
                    CLI_NOMBRE = cl.CLI_NOMBRE,
                    CRT_DOCTRAN = d.DDO_DOCTRAN,
                    CRT_MONTO = ca.CRT_MONTO,
                    SALDO = d.DDO_MONTO - (d.DDO_CANCELA ?? 0),
                    CRT_CANCELA = ca.CRT_CANCELA,
                    CRT_CANCELA_CH = ca.CRT_CANCELA_CH
                }
            )

            .ToListAsync();


            if (resultados == null || !resultados.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA REPORTES";
                return response;
            }

            response.Data = resultados;
            response.Success = true;
            response.Message = "REPORTES ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetReportexNumeroAsync() " + ex.ToString() + cartera);
        }
        return response;
    }


    public async Task<ServiceResponse<object>> CierreReporteAsync(AuxGeneral auxGeneral)
    {
        var response = new ServiceResponse<object>();
        try
        {
            if (auxGeneral == null || auxGeneral.AuxDecimal <= 0 || auxGeneral.AGE_CODIGO <= 0)
            {
                response.Data = null;
                response.Success = false;
                response.Message = "Los datos proporcionados son nulos o inválidos.";
                return response;
            }


            await EnviaReporteAsync(auxGeneral.AuxDecimal ?? 0, auxGeneral.AGE_CODIGO ?? 0);

            var registros = await _context.CARTERA
                        .Where(c => c.CRT_NUMERO == auxGeneral.AuxDecimal
                        && c.CRT_AGENTE == auxGeneral.AGE_CODIGO && c.CRT_ESTADO == 2)
                        .ToListAsync();



            if (registros != null || registros.Any())
            {
                response.Success = true;
                response.Message = "REPORTE CERRADO CORRECTAMENTE";
            }
            else

            {
                response.Success = true;
                response.Message = "NO SE CERRÓ EL REPORTE VERIFICAR CON EL DEPARTAMENTO DE CARTERA";
            }



        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ CierreReporteAsync() " + ex.ToString() + auxGeneral);
        }
        return response;
    }


    public async Task EnviaReporteAsync(decimal numero, decimal agente)
    {
        try
        {
            // Crear los parámetros para el procedimiento almacenado
            var paramClienteCodigo = new OracleParameter("P_NUMERO", OracleDbType.Decimal)
            {
                Value = numero,
                Direction = ParameterDirection.Input
            };

            var paramUsuario = new OracleParameter("P_USR_ID", OracleDbType.Decimal)
            {
                Value = agente,
                Direction = ParameterDirection.Input
            };

            // Ejecutar el procedimiento almacenado
            await _context.Database.ExecuteSqlRawAsync(
                "BEGIN DATA_USR.P_CXC_ENVIA_REPORTE(:P_NUMERO, :P_USR_ID); END;",
                paramClienteCodigo,
                paramUsuario
            );

            Console.WriteLine("Procedimiento ejecutado exitosamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al ejecutar el procedimiento almacenado: {ex.Message}");
            _logger.LogError(" --------------------- ERROR ------------------ EnviaReporteAsync() " + ex.ToString() + numero + agente);
            throw;
        }
    }



    public async Task<ServiceResponse<object>> GetCarteraCompletaAsync(decimal agente)
    {
        var response = new ServiceResponse<object>();
        try
        {

            var preciosPermitidosNav = new HashSet<decimal>();
            if (!string.IsNullOrEmpty(P_LISTA_PNAV))
            {
                preciosPermitidosNav = P_LISTA_PNAV.Split(';')
                                                   .Select(s => decimal.TryParse(s.Trim(), out var dec) ? dec : (decimal?)null)
                                                   .Where(d => d.HasValue)
                                                   .Select(d => d.Value)
                                                   .ToHashSet();
            }

            var hoy = DateTime.Now;

            var resultado = await (_context.REP_CART_VEN_INT_T
                  .Where(p =>


        (

            (

                p.AGE_CODIGO == agente
                &&
                (
                    (!p.DOC.StartsWith("NCC") || (p.DOC.StartsWith("NCC") && p.DDO_FECHA_VEN >= hoy.AddDays(-8)))
                    &&
                    (!p.DOC.StartsWith("REC") || (p.DOC.StartsWith("REC") && p.DDO_FECHA_VEN >= hoy.AddDays(-5)))
                )
            )
            ||

            (

                p.AGE_CODIGO == agente
                &&
                 preciosPermitidosNav.Contains(p.CLI_LISTAPRE ?? 0) //. == 90000183
                &&
                (
                    (!p.DOC.StartsWith("NCC") || (p.DOC.StartsWith("NCC") && p.DDO_FECHA_VEN >= hoy.AddDays(-90)))
                    &&
                    (!p.DOC.StartsWith("REC") || (p.DOC.StartsWith("REC") && p.DDO_FECHA_VEN >= hoy.AddDays(-90)))
                )
            )
        )
    )

                .Select(p => new
                {
                    p.CLI_NOMBRE,
                    p.CUE_NOMBRE,
                    p.DOC,
                    p.DDO_FECHA_EMI,
                    p.DDO_FECHA_VEN,
                    p.DDO_MONTO,
                    p.SALDO
                })
                .Distinct()
                .OrderBy(x => x.CLI_NOMBRE)
                .ThenBy(x => x.DDO_FECHA_EMI)
                ).ToListAsync();


            if (resultado == null || !resultado.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "NO SE ENCUENTRA DEUDAS PARA EL CLIENTE SELECCIONADO";
                return response;
            }

            response.Data = resultado;
            response.Success = true;
            response.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.ToString();
            response.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetCarteraCompletaAsync() " + ex.ToString() + agente);
        }
        return response;
    }

    #region Notas de Credito

    public async Task<ServiceResponse<object>> GetClientesNcsAsync(decimal agente)
    {

        try
        {
            var referencias = await _context.REP_REFERENCIAS_DEV_INFOA
                            .Where(r => (r.CCO_AGENTE == agente))
                            //r.CMO_REFERENCIA.ToUpper().Contains("AL"))
                            .Select(r => new
                            {
                                CLI_NOMBRE = r.CLI_NOMBRE,
                                CMO_REFERENCIA = r.CMO_REFERENCIA,
                                CMO_CCO_COMPROBA = r.CCO_CODIGO,
                                REFERENCIA = r.CMO_REFERENCIA
                                
                            })
                            .ToListAsync();


            if (referencias == null || !referencias.Any())
            {
                respuesta.Data = null;
                respuesta.Success = true;
                respuesta.Message = "NO SE ENCUENTRA DATOS PARA EL AGENTE SELECCIONADO";
                return respuesta;
            }

            respuesta.Data = referencias;
            respuesta.Success = true;
            respuesta.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            respuesta.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetClientesNcsAsync() " + ex.ToString() + agente);
        }
        return respuesta;

    }


    public async Task<ServiceResponse<object>> GetNcsxCodigoAsync(decimal codigo)
    {
        try
        {
            var totalValues = await _context.TOTAL
                            .Where(t => t.TOT_CCO_COMPROBA == codigo)
                            .Select(t => t.TOT_TOTAL)
                            .ToListAsync();

            var totalDefault = totalValues.FirstOrDefault();


            var ccomData = await (
                from ccom in _context.CCOMPROBA
                join cmov in _context.CMOVINV on ccom.CCO_CODIGO equals cmov.CMO_CCO_COMPROBA into cmovJoin
                from cmov in cmovJoin.DefaultIfEmpty()
                join cliente in _context.CLIENTE on ccom.CCO_CODCLIPRO equals cliente.CLI_CODIGO
                where ccom.CCO_EMPRESA == 1 && ccom.CCO_CODIGO == codigo
                select new
                {
                    CcoEmpresa = ccom.CCO_EMPRESA,
                    CcoCodigo = ccom.CCO_CODIGO,
                    ClienteNombre = cliente.CLI_NOMBRE,
                    Referencia = cmov.CMO_REFERENCIA,
                    TotalNc = (
                        from nc in _context.VL_NCC_IDC_LDV
                        where nc.ID_EMPRESA_NR == ccom.CCO_EMPRESA && nc.ID_IDC_NR == ccom.CCO_CODIGO
                        select nc.TOTAL_NOTA_CREDITO ?? 0
                    ).Sum() == 0 ? totalDefault : (
                        from nc in _context.VL_NCC_IDC_LDV
                        where nc.ID_EMPRESA_NR == ccom.CCO_EMPRESA && nc.ID_IDC_NR == ccom.CCO_CODIGO
                        select nc.TOTAL_NOTA_CREDITO ?? 0
                    ).Sum()
                }
            ).ToListAsync();

            // Luego realiza la proyección y la llamada al método asíncrono fuera del contexto de EF
            var result = new List<object>();
            foreach (var item in ccomData)
            {
                var doc = await ObtenerNumeroComprobanteAsync(item.CcoEmpresa, item.CcoCodigo);
                result.Add(new
                {
                    NUMERO_DOCUMENTO = doc,
                    CLIENTE = item.ClienteNombre,
                    REFERENCIA = item.Referencia,
                    TOTAL_NC = item.TotalNc
                });
            }


            if (result == null || !result.Any())
            {
                respuesta.Data = null;
                respuesta.Success = true;
                respuesta.Message = "NO SE ENCUENTRA DATOS PARA LA REFERENCIA SELECCIONADA";
            }

            respuesta.Data = result;
            respuesta.Success = true;
            respuesta.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            respuesta.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetNcsxCodigoAsync() " + ex.ToString() + codigo);
        }
        return respuesta;
    }


    public async Task<ServiceResponse<object>> GetDetalleNcsxCodigoAsync(decimal codigo)
    {
        try
        {
            var result = await _context.VL_NCC_IDC_LDV
                                .Where(v => v.ID_IDC_NR == codigo)
                                .ToListAsync();


            if (result == null || !result.Any())
            {
                respuesta.Data = null;
                respuesta.Success = true;
                respuesta.Message = "NO SE ENCUENTRA DATOS PARA EL AGENTE SELECCIONADO";
            }

            respuesta.Data = result;
            respuesta.Success = true;
            respuesta.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            respuesta.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetDetalleNcsxCodigoAsync() " + ex.ToString() + codigo);
        }
        return respuesta;
    }


    public async Task<ServiceResponse<object>> GetDevolicionxCodigoAsync(decimal codigo)
    {
        try
        {
            var _result = await (
                                from ccom in _context.CCOMPROBA
                                join cmov in _context.CMOVINV on ccom.CCO_CODIGO equals cmov.CMO_CCO_COMPROBA
                                join cliente in _context.CLIENTE on ccom.CCO_CODCLIPRO equals cliente.CLI_CODIGO
                                where ccom.CCO_EMPRESA == p_empresa && ccom.CCO_CODIGO == codigo
                                select new
                                {
                                    ccom.CCO_EMPRESA,
                                    ccom.CCO_CODIGO,
                                    Cliente = cliente.CLI_NOMBRE,
                                    Referencia = cmov.CMO_REFERENCIA,
                                    Total = (
                                        from nc in _context.VL_NCC_IDC_LDV
                                        where nc.ID_EMPRESA_NR == ccom.CCO_EMPRESA && nc.ID_IDC_NR == ccom.CCO_CODIGO
                                        select nc.TOTAL_NOTA_CREDITO ?? 0
                                    ).Sum()
                                }
                            ).ToListAsync();

            // Añade `Numero` después de obtener los datos
            var result = new List<object>();
            foreach (var item in _result)
            {
                var numero = await ObtenerNumeroComprobanteAsync(item.CCO_EMPRESA, item.CCO_CODIGO);
                result.Add(new
                {
                    NUMERO_DOCUMENTO = numero,
                    CLIENTE = item.Cliente,
                    REFERENCIA = item.Referencia,
                    TOTAL_NC = item.Total
                });
            }



            if (result == null || !result.Any())
            {
                respuesta.Data = null;
                respuesta.Success = true;
                respuesta.Message = "NO SE ENCUENTRA DATOS PARA EL AGENTE SELECCIONADO";
            }

            respuesta.Data = result;
            respuesta.Success = true;
            respuesta.Message = "DOCUMENTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            respuesta.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetDevolicionxCodigoAsync() " + ex.ToString() + codigo);
        }
        return respuesta;
    }


    public async Task<ServiceResponse<object>> GetDevProductosAproxCodigoAsync(decimal codigo)
    {
        try
        {
            var result = await _context.LST_PRODUCTOS_APR_NCC
                                .Where(v => v.CCO_CODIGO == codigo)
                                .ToListAsync();


            if (result == null || !result.Any())
            {
                respuesta.Data = null;
                respuesta.Success = true;
                respuesta.Message = "NO SE ENCUENTRA DATOS PARA EL DOCUMENTO SELECCIONADO";
            }

            respuesta.Data = result;
            respuesta.Success = true;
            respuesta.Message = "PRODUCTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            respuesta.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetDevProductosAproxCodigoAsync() " + ex.ToString() + codigo);
        }
        return respuesta;
    }

    public async Task<ServiceResponse<object>> GetDevProductosDenxCodigoAsync(decimal codigo) // trae los productos denegados de una devolución.
    {
        try
        {
            var _result = await (from cc in _context.CCOMPROBA
                                 join dm in _context.DMOVINVI on cc.CCO_CODIGO equals dm.DMO_CMO_COMPROBA
                                 join pr in _context.PRODUCTO on dm.DMO_PRODUCTO equals pr.PRO_CODIGO
                                 join um in _context.UMEDIDA on dm.DMO_UDIGITADA equals um.UMD_CODIGO
                                 join td in _context.TIPODEV on dm.DMO_TIPODEV equals td.TDE_CODIGO
                                 join dc in _context.DEVOLUCION_CAB on cc.CCO_CODIGO equals dc.DEV_DOC_REFERENCIA


                                 join dd in _context.DEVOLUCION_DET on new
                                 {

                                     DevCodigo = (decimal)dc.DEV_CODIGO,
                                     ProductCode = (decimal)dm.DMO_PRODUCTO
                                 }
                                 equals new
                                 {

                                     DevCodigo = (decimal)dd.DVD_CODIGO,
                                     ProductCode = (decimal?)dd.DVD_PRODUCTO ?? 0
                                 }

                                 where cc.CCO_CODIGO == codigo

                                 select new
                                 {
                                     pr.PRO_ID,
                                     pr.PRO_NOMBRE,
                                     um.UMD_ID,
                                     dm.DMO_CDIGITADA,
                                     td.TDE_NOMBRE, // MOTIVO
                                     dm.DMO_EMPRESA,
                                     dm.DMO_FAC_COMPROBA,
                                     dd.DVD_OBSERVACION_CALIDAD2

                                 }
                               ).ToListAsync();


            var result = new List<object>();
            foreach (var item in _result)
            {
                var numero = await ObtenerNumeroComprobanteAsync(item.DMO_EMPRESA, item.DMO_FAC_COMPROBA);
                result.Add(new
                {
                    FACTURA = numero,
                    ID = item.PRO_ID,
                    PRODUCTO = item.PRO_NOMBRE,
                    UNIDAD = item.UMD_ID,
                    CANTIDAD = item.DMO_CDIGITADA,
                    MOTIVO = item.TDE_NOMBRE,
                    DVD_OBSERVACION_CALIDAD2 = item.DVD_OBSERVACION_CALIDAD2
                });
            }


            if (result == null || !result.Any())
            {
                respuesta.Data = null;
                respuesta.Success = true;
                respuesta.Message = "NO SE ENCUENTRA DATOS PARA EL DOCUMENTO SELECCIONADO";
            }

            respuesta.Data = result;
            respuesta.Success = true;
            respuesta.Message = "PRODUCTOS ENCONTRADOS EXISTOSAMENTE";
        }
        catch (Exception ex)
        {
            respuesta.Success = false;
            respuesta.Message = ex.ToString();
            respuesta.Data = null;
            _logger.LogError(" --------------------- ERROR ------------------ GetDevProductosDenxCodigoAsync() " + ex.ToString() + codigo);
        }
        return respuesta;
    }

    public async Task<string?> ObtenerNumeroComprobanteAsync(int cco_empresa, decimal cco_codigo)
    {
        // Aumenta el tamaño del parámetro para evitar errores de tamaño
        var resultadoParam = new OracleParameter("resultado", OracleDbType.Clob, ParameterDirection.Output);

        // Ejecuta el comando
        await _context.Database.OpenConnectionAsync(); // Mantiene la conexión abierta mientras se lee el CLOB
        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "BEGIN :resultado := ast_gen.numero_comprobante(:cco_empresa, :cco_codigo); END;",
                new OracleParameter("cco_empresa", OracleDbType.Int32) { Value = cco_empresa },
                new OracleParameter("cco_codigo", OracleDbType.Decimal) { Value = cco_codigo },
                resultadoParam);

            // Lee el valor antes de cerrar la conexión
            using (var reader = resultadoParam.Value as OracleClob)
            {
                if (reader != null && !reader.IsNull)
                {
                    return reader.Value; // Obtiene el valor antes de cerrar la conexión
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(" --------------------- ERROR ------------------ ObtenerNumeroComprobanteAsync() " + ex.ToString() + cco_empresa + cco_codigo);
        }
        finally
        {
            await _context.Database.CloseConnectionAsync(); // Cierra la conexión una vez que se ha leído el valor
        }

        return null;
    }





    #endregion


}
