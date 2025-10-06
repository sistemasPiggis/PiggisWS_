using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Services.Utils;
using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;

namespace PIGGISWS.Services;

public class ClientesService : IClientesService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IAgenteService _agenteService;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Cliente cliente = new Cliente();
    List<Cliente> clientes = new List<Cliente>();
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    List<Cliente_Dia_Gestion_Nuevo> listaClientesDia = new List<Cliente_Dia_Gestion_Nuevo>();


    int p_empresa = 0;
    int p_cli_Tipo = 0;
    int p_cli_Bloqueo = 0;
    int p_cli_Inactivo = 0;
    int p_cli_cupo = 0;
    string p_cli_estado = "";
    string P_CLI_NOT_MAILS;
    string P_LISTA_PNAV = "";
    int P_NAVIDAD = 0;
    public ClientesService(ApplicationDbContext context, IEmailService emailService, IAgenteService agenteService)
    {
        _context = context;
        P_CLI_NOT_MAILS = string.Empty;
        _emailService = emailService;
        GetParametros();
        _agenteService = agenteService;
    }

    public void GetParametros()
    {
        parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "ClientesService" || p.SERVICIO == "GENERAL").ToList();
        p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
        p_cli_Tipo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 4)?.VALOR ?? "0");
        p_cli_Bloqueo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 5)?.VALOR ?? "0");
        p_cli_Inactivo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 6)?.VALOR ?? "0");
        p_cli_cupo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 7)?.VALOR ?? "0");
        p_cli_estado = parametros.First(p => p.CODIGO == 8)?.VALOR ?? "";
        P_CLI_NOT_MAILS = parametros.First(p => p.CODIGO == 19)?.VALOR ?? "";
        P_LISTA_PNAV = parametros.First(p => p.CODIGO == 61)?.VALOR ?? "";
        P_NAVIDAD = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 60)?.VALOR ?? "0");
    }


    public async Task<ServiceResponse<object>> GetClientesxAgente(int agente)
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




        var preciosPermitidosNav = new HashSet<decimal>();
        if (!string.IsNullOrEmpty(P_LISTA_PNAV))
        {
            preciosPermitidosNav = P_LISTA_PNAV.Split(';')
                                               .Select(s => decimal.TryParse(s.Trim(), out var dec) ? dec : (decimal?)null)
                                               .Where(d => d.HasValue)
                                               .Select(d => d.Value)
                                               .ToHashSet();
        }



        try
        {

            var query = from cl in _context.CLIENTE
                        join p in _context.POLITICA on cl.CLI_POLITICAS equals p.POL_CODIGO
                        join ce in _context.CLIENTE_EXT on cl.CLI_CODIGO equals ce.CLI_CODIGO
                        // LEFT JOIN con CLIENTE_DIA para evitar duplicados
                        join cd_join in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd_join.CDI_CLIENTE into cd_group
                        from cd in cd_group.DefaultIfEmpty()
                        let deuda = _context.F_CXC_SALDO_CARTERA_PED_ST_NR(cl.CLI_EMPRESA, cl.CLI_CODIGO)
                        where cl.CLI_EMPRESA == p_empresa
                              && cl.CLI_TIPO == p_cli_Tipo
                              && cl.CLI_INACTIVO == p_cli_Inactivo

                              //&& cl.CLI_BLOQUEO == p_cli_Bloqueo
                              && cl.CLI_AGENTE == agente
                              && (
                                  diasPermitidos.Contains(dayformateado)
                                  || cd.CDI_DIA == dayformateado
                              )

                            && (preciosPermitidosNav.Count == 0 || !preciosPermitidosNav.Contains(cl.CLI_LISTAPRE ?? -1))
                        // Agrupamos por cliente para obtener resultados únicos
                        group new { cl, p, ce, cd } by new { cl.CLI_CODIGO, cl.CLI_NOMBRE, 
                            cl.CLI_AGENTE, cl.CLI_ID, cl.CLI_LISTAPRE, cl.CLI_ILIMITADO, cl.CLI_ZONA, 
                            cl.CLI_TELEFONO1, cl.CLI_POLITICAS, cl.CLI_DIRECCION, cl.CLI_DIR_ENTREGA, 
                            cl.CLI_NOMBRECOM, cl.CLI_MAIL, cl.CLI_RUC_CEDULA, ce.ID_PROVINCIA_FK, ce.ID_CANTON_FK, 
                            cl.CLI_ESTABLECIMIENTO, p.POL_PORC_DESC, p.POL_PORC_FINANC, p.POL_PORC_PRO_PAGO, p.POL_PORC_PAG_CONTA, 
                            p.POL_LINEA_CREDITO, p.POL_DIAS_PLAZO, p.POL_NRO_PAGOS, cl.CLI_PARROQUIA, cl.CLI_CUPO, cl.CLI_BLOQUEO, 
                            ce.CLI_LATITUD_NR, ce.CLI_LONGITUD_NR, ce.CLI_LATITUD1_NR, ce.CLI_LONGITUD1_NR, deuda } into g
                        select new ClienteDto
                        {
                            // Tomamos los datos de la clave de agrupación (g.Key)
                            CLI_EMPRESA = p_empresa, // O desde donde corresponda
                            CLI_CODIGO = g.Key.CLI_CODIGO,
                            CLI_NOMBRE = g.Key.CLI_NOMBRE,
                            CLI_AGENTE = g.Key.CLI_AGENTE,
                            CLI_ID = g.Key.CLI_ID,
                            CLI_LISTAPRE = g.Key.CLI_LISTAPRE ?? 0,
                            CLI_ILIMITADOF = g.Key.CLI_ILIMITADO ?? 0,
                            CLI_ZONA = g.Key.CLI_ZONA ?? 0,
                            // Si es un día permitido, se asigna ese día. Si no, se busca si el cliente tiene visita ese día.
                            CDI_DIA = diasPermitidos.Contains(dayformateado) ? dayformateado : (g.FirstOrDefault(x => x.cd != null && x.cd.CDI_DIA == dayformateado) != null ? dayformateado : null),
                            CLI_TELEFONO1 = g.Key.CLI_TELEFONO1,
                            CLI_POLITICAS = g.Key.CLI_POLITICAS,
                            CLI_DIRECCION = g.Key.CLI_DIRECCION,
                            CLI_DIR_ENTREGA = g.Key.CLI_DIR_ENTREGA,
                            CLI_NOMBRECOM = g.Key.CLI_NOMBRECOM,
                            CLI_MAIL = g.Key.CLI_MAIL,
                            CLI_RUC_CEDULA = g.Key.CLI_RUC_CEDULA,
                            ID_PROVINCIA_FK = g.Key.ID_PROVINCIA_FK ?? 0,
                            ID_CANTON_FK = g.Key.ID_CANTON_FK ?? 0,
                            CLI_ESTABLECIMIENTO = g.Key.CLI_ESTABLECIMIENTO,
                            POL_PORC_DESC = g.Key.POL_PORC_DESC,
                            POL_PORC_FINANC = g.Key.POL_PORC_FINANC,
                            POL_PORC_PRO_PAGO = g.Key.POL_PORC_PRO_PAGO,
                            POL_PORC_PAG_CONTA = g.Key.POL_PORC_PAG_CONTA,
                            POL_LINEA_CREDITO = g.Key.POL_LINEA_CREDITO,
                            POL_DIAS_PLAZO = g.Key.POL_DIAS_PLAZO,
                            POL_NRO_PAGOS = g.Key.POL_NRO_PAGOS,
                            CLI_PARROQUIA = g.Key.CLI_PARROQUIA,
                            CUPO = g.Key.CLI_CUPO,
                            CLI_BLOQUEO = g.Key.CLI_BLOQUEO ?? 0,
                            CLI_LATITUD_NR = g.Key.CLI_LATITUD_NR ?? 0M,
                            CLI_LONGITUD_NR = g.Key.CLI_LONGITUD_NR ?? 0M,
                            CLI_LATITUD1_NR = g.Key.CLI_LATITUD1_NR ?? 0M,
                            CLI_LONGITUD1_NR = g.Key.CLI_LONGITUD1_NR ?? 0M,
                            DEUDA = g.Key.deuda,
                            FECHA_SUG = _context.F_VNT_FECHA_FACTURAR_DT(1, g.Key.CLI_CODIGO, g.Key.CLI_AGENTE ?? 0),
                            DISPONIBLE = (g.Key.CLI_CUPO ?? 0) - g.Key.deuda
                        }; 

            var clientes = await query.OrderBy(x => x.CLI_NOMBRE).ToListAsync();


            if (clientes == null || !clientes.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = FormatosTexto.DatosNoEncontrados;
            }
            else
            {
                //foreach (var cliente in clientes)
                //{
                //    cliente.DEUDA = await ObtenerDeudaAsync(cliente.CLI_EMPRESA, cliente.CLI_CODIGO);
                //    cliente.DISPONIBLE = (cliente.CUPO ?? 0) - (cliente.DEUDA);
                //    cliente.FECHA_SUG = await ObtenerFechaSugeridaAsync(cliente.CLI_EMPRESA, cliente.CLI_CODIGO, cliente.CLI_AGENTE ?? 0);
                //}

                response.Data = clientes;
                response.Success = true;
                response.Message = "Clientes encontrados exitosamente.";
            }
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            // Log the exception details (ex) here as needed
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }

    public async Task<ServiceResponse<object>> CreateClienteAsync(AuxClientesNuevos clientes_Nuevos)
    {

        var response = new ServiceResponse<object>();
        int longitud_cedula = clientes_Nuevos.Clientes_Nuevos.CI_RUC.Length;
        int tipo_identificacion;
        int _ID_SECUENCIA_PK;

        string ageresponse = await _agenteService.GetUsuarioAsync(clientes_Nuevos.Clientes_Nuevos.ID_AGENTE_FK);
        if (string.IsNullOrEmpty(ageresponse))
        {
            ageresponse = "DATA_USR";
        }

        response = await ValidaClienteNuevoxCedRucAsync(clientes_Nuevos.Clientes_Nuevos.CI_RUC, clientes_Nuevos.Clientes_Nuevos.DIRECCION_ENTREGA);
        if (response.Data != null)
        {


            return response;
        }

        switch (longitud_cedula)
        {
            case 10:
                tipo_identificacion = 1;
                break;
            case 13:
                tipo_identificacion = 2;
                break;
            default:
                tipo_identificacion = 0; /// error al ingreso de cedula
                break;
        }
        if (tipo_identificacion != 0)
        {


            try
            {

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT CLIENTES_NUEVOS_SEQ.NEXTVAL FROM dual";
                    await _context.Database.OpenConnectionAsync();
                    var result = await command.ExecuteScalarAsync();
                    _ID_SECUENCIA_PK = Convert.ToInt32(result);
                }


                var cliente = new Clientes_Nuevos
                {
                    ID_EMPRESA_FK = p_empresa,
                    ID_SECUENCIA_PK = _ID_SECUENCIA_PK,
                    ID_AGENTE_FK = clientes_Nuevos.Clientes_Nuevos.ID_AGENTE_FK,
                    CI_RUC = clientes_Nuevos.Clientes_Nuevos.CI_RUC,
                    NOMBRE_CLIENTE = clientes_Nuevos.Clientes_Nuevos.NOMBRE_CLIENTE,
                    NOMBRE_COMERCIAL = clientes_Nuevos.Clientes_Nuevos.NOMBRE_COMERCIAL,
                    TELEFONO = clientes_Nuevos.Clientes_Nuevos.TELEFONO,
                    EMAIL = clientes_Nuevos.Clientes_Nuevos.EMAIL,
                    ID_PROVINCIA_FK = clientes_Nuevos.Clientes_Nuevos.ID_PROVINCIA_FK,
                    ID_CANTON_FK = clientes_Nuevos.Clientes_Nuevos.ID_CANTON_FK,
                    ID_ZONA_FK = clientes_Nuevos.Clientes_Nuevos.ID_ZONA_FK,
                    DIRECCION_CLIENTE = clientes_Nuevos.Clientes_Nuevos.DIRECCION_CLIENTE,
                    ID_ESTABLECIMIENTO_FK = clientes_Nuevos.Clientes_Nuevos.ID_ESTABLECIMIENTO_FK,
                    ID_LISTA_PRECIO_FK = clientes_Nuevos.Clientes_Nuevos.ID_LISTA_PRECIO_FK,
                    ESTADO = p_cli_estado,
                    ID_PARROQUIA_FK = clientes_Nuevos.Clientes_Nuevos.ID_PARROQUIA_FK,
                    DIRECCION_ENTREGA = clientes_Nuevos.Clientes_Nuevos.DIRECCION_ENTREGA,
                    TIPO_IDENTIFICACION = tipo_identificacion,
                    LATITUD_NR = clientes_Nuevos.Clientes_Nuevos.LATITUD_NR,
                    LONGITUD_NR = clientes_Nuevos.Clientes_Nuevos.LONGITUD_NR,
                    CREA_USR = ageresponse, 
                    MOD_USR = ageresponse,
                    CREA_FECHA = DateTime.Now, 
                    
                };

                _context.CLIENTES_NUEVOS.Add(cliente);
                _context.SaveChanges();

                var idPadreGenerado = cliente.ID_SECUENCIA_PK;
                if (idPadreGenerado <= 0)
                {
                    response.Message = "No se pudo insertar datos de cliente";
                    response.Data = null;
                    response.Success = false;
                }

               

                foreach (var nuevoCliente in clientes_Nuevos.Cliente_Dia_Gestion_Nuevo)
                {
                    using (var command = _context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = "SELECT CLIENTE_DIA_GESTION_NUEVO_SEQ.NEXTVAL FROM dual";
                        await _context.Database.OpenConnectionAsync();
                        var result = await command.ExecuteScalarAsync();
                        _ID_SECUENCIA_PK = Convert.ToInt32(result);
                    }
                    var nuevoClienteDia = new Cliente_Dia_Gestion_Nuevo
                    {
                        ID_SECUENCIA_PK = _ID_SECUENCIA_PK,
                        ID_EMPRESA_FK = p_empresa,
                        TIPO_GESTION_TX = nuevoCliente.TIPO_GESTION_TX,
                        ID_CLIENTE_NUEVO_FK = cliente.CI_RUC,
                        DIA_NR = nuevoCliente.DIA_NR,
                        INACTIVO_NR = nuevoCliente.INACTIVO_NR,
                        DIRECCION_CLIENTE = nuevoCliente.DIRECCION_CLIENTE,
                        DIRECCION_ENTREGA = nuevoCliente.DIRECCION_ENTREGA, 
                        CREA_FECHA = DateTime.Now,
                        CREA_USR = ageresponse,
                        MOD_FECHA = DateTime.Now,
                        MOD_USR = ageresponse
                    };

                    listaClientesDia.Add(nuevoClienteDia);
                }

                _context.CLIENTE_DIA_GESTION_NUEVO.AddRange(listaClientesDia);
                await _context.SaveChangesAsync();


                response.Message = "Registro creado exitosamente";
                response.Data = cliente;
                response.Success = true;
                return response;

            }
            catch (Exception ex)
            {
                response.Message += "NO SE PUDO CREAR EL CLIENTE " + ex.Message;
                response.Data = null;
                response.Success = true;
                return response;
            }
        }
        else
        {
            response.Message = "Cédula mal digitada";
            response.Data = null;
            response.Success = true;
        }
        return response;
    }


    public async Task<ServiceResponse<object>> GetClientexCodigo(long cli_codigo)
    {

        // Crea un objeto CultureInfo en español
        CultureInfo ci = new CultureInfo("es-ES");

        var response = new ServiceResponse<object>();

        try
        {
            var clientes = await (from cl in _context.CLIENTE
                                  join p in _context.POLITICA on cl.CLI_POLITICAS equals p.POL_CODIGO
                                  join ce in _context.CLIENTE_EXT on cl.CLI_CODIGO equals ce.CLI_CODIGO
                                  //join cd in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd.CDI_CLIENTE into cdGroup
                                  //from cd in cdGroup.DefaultIfEmpty()
                                  where cl.CLI_EMPRESA == p_empresa
                                        && cl.CLI_TIPO == p_cli_Tipo
                                        && cl.CLI_INACTIVO == p_cli_Inactivo
                                        && cl.CLI_BLOQUEO == p_cli_Bloqueo
                                        && cl.CLI_CODIGO == cli_codigo
                                  select new ClienteDto
                                  {
                                      CLI_EMPRESA = cl.CLI_EMPRESA,
                                      CLI_CODIGO = cl.CLI_CODIGO,
                                      CLI_NOMBRE = cl.CLI_NOMBRE,
                                      CLI_AGENTE = cl.CLI_AGENTE,
                                      CLI_ID = cl.CLI_ID,
                                      CLI_LISTAPRE = cl.CLI_LISTAPRE ?? 0,
                                      CLI_ILIMITADOF = cl.CLI_ILIMITADO ?? 0,
                                      CLI_ZONA = cl.CLI_ZONA ?? 0,
                                      CLI_TELEFONO1 = cl.CLI_TELEFONO1,
                                      CLI_POLITICAS = cl.CLI_POLITICAS,
                                      CLI_DIRECCION = cl.CLI_DIRECCION,
                                      CLI_DIR_ENTREGA = cl.CLI_DIR_ENTREGA,
                                      CLI_NOMBRECOM = cl.CLI_NOMBRECOM,
                                      CLI_MAIL = cl.CLI_MAIL,
                                      CLI_RUC_CEDULA = cl.CLI_RUC_CEDULA,
                                      ID_PROVINCIA_FK = ce.ID_PROVINCIA_FK ?? 0,
                                      ID_CANTON_FK = ce.ID_CANTON_FK ?? 0,
                                      CLI_ESTABLECIMIENTO = cl.CLI_ESTABLECIMIENTO,
                                      POL_PORC_DESC = p.POL_PORC_DESC,
                                      POL_PORC_FINANC = p.POL_PORC_FINANC,
                                      POL_PORC_PRO_PAGO = p.POL_PORC_PRO_PAGO,
                                      POL_PORC_PAG_CONTA = p.POL_PORC_PAG_CONTA,
                                      POL_LINEA_CREDITO = p.POL_LINEA_CREDITO,
                                      POL_DIAS_PLAZO = p.POL_DIAS_PLAZO,
                                      POL_NRO_PAGOS = p.POL_NRO_PAGOS,
                                      CLI_PARROQUIA = cl.CLI_PARROQUIA,
                                      CUPO = cl.CLI_CUPO

                                  })
                 .OrderBy(x => x.CLI_NOMBRE)
                 .ToListAsync();

            var cliente = clientes.FirstOrDefault();
            if (cliente != null)
            {
                cliente.DEUDA = await ObtenerDeudaAsync(p_empresa, cli_codigo);
                cliente.FECHA_SUG = await ObtenerFechaSugeridaAsync(p_empresa, cli_codigo, cliente.CLI_AGENTE ?? 0);
            }

            if (cliente == null)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "Cliente no encontrado.";
                return response;
            }

            response.Data = cliente;
            response.Success = true;
            response.Message = "Clientes encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }


    public async Task<ServiceResponse<object>> EditClienteAsync(Cliente_ClienteExt cliente)
    {

        var response = new ServiceResponse<object>();

        try
        {
            if (cliente != null && cliente.Cliente != null)
            {
                var _singlecliente_ = await (_context.CLIENTE.Where(c => c.CLI_CODIGO == cliente.Cliente.CLI_CODIGO).ToListAsync());
                var _singlecliente = _singlecliente_.FirstOrDefault();
                var _clienteExt_ = await (_context.CLIENTE_EXT.Where(ce => ce.CLI_CODIGO == cliente.Cliente.CLI_CODIGO).ToListAsync());
                var _clienteExt = _clienteExt_.FirstOrDefault();


                if (_singlecliente == null)
                {
                    response.Data = null;
                    response.Success = true;
                    response.Message = "No se encontraron clientes.";
                    return response;
                }

                var oldValues = new
                {
                    Nombre = _singlecliente?.CLI_NOMBRE,
                    Direccion = _singlecliente?.CLI_DIRECCION,
                    DireccionEntrega = _singlecliente?.CLI_DIR_ENTREGA,
                    Email = _singlecliente?.CLI_MAIL,
                    Establecimiento = _singlecliente?.CLI_ESTABLECIMIENTO,
                    Parroquia = _singlecliente?.CLI_PARROQUIA,
                    Provincia = _clienteExt?.ID_PROVINCIA_FK,
                    Canton = _clienteExt?.ID_CANTON_FK
                };



                _singlecliente.CLI_DIRECCION = cliente.Cliente.CLI_DIRECCION;
                _singlecliente.CLI_DIR_ENTREGA = cliente.Cliente.CLI_DIR_ENTREGA;
                _singlecliente.CLI_MAIL = cliente.Cliente.CLI_MAIL;

                _singlecliente.CLI_PARROQUIA = cliente.Cliente.CLI_PARROQUIA;
                _clienteExt.ID_PROVINCIA_FK = cliente.Cliente_Ext?.ID_PROVINCIA_FK;
                _clienteExt.ID_CANTON_FK = cliente.Cliente_Ext?.ID_CANTON_FK;
                _singlecliente.CLI_ESTABLECIMIENTO = cliente.Cliente.CLI_ESTABLECIMIENTO;
                _context.Entry(_singlecliente).Property(x => x.CLI_DIRECCION).IsModified = true;
                _context.Entry(_singlecliente).Property(x => x.CLI_DIR_ENTREGA).IsModified = true;
                _context.Entry(_singlecliente).Property(x => x.CLI_MAIL).IsModified = true;
                _context.Entry(_singlecliente).Property(x => x.CLI_PARROQUIA).IsModified = true;
                _context.Entry(_singlecliente).Property(x => x.CLI_ESTABLECIMIENTO).IsModified = true;
                await _context.SaveChangesAsync();


                var updatedValues = new
                {
                    Nombre = _singlecliente.CLI_NOMBRE,
                    Direccion = _singlecliente.CLI_DIRECCION,
                    DireccionEntrega = _singlecliente.CLI_DIR_ENTREGA,
                    Email = _singlecliente.CLI_MAIL,
                    Establecimiento = _singlecliente.CLI_ESTABLECIMIENTO,
                    Parroquia = _singlecliente.CLI_PARROQUIA,
                    Provincia = _clienteExt.ID_PROVINCIA_FK,
                    Canton = _clienteExt.ID_CANTON_FK
                };

                var sb = new StringBuilder();
                sb.AppendLine("Se han actualizado los datos del cliente con los siguientes cambios:");
                sb.AppendLine();

                if (oldValues.Direccion != updatedValues.Direccion)
                    sb.AppendLine($"Dirección De: {oldValues.Direccion}  -> A: {updatedValues.Direccion}");
                if (oldValues.DireccionEntrega != updatedValues.DireccionEntrega)
                    sb.AppendLine($"Dirección de Entrega De: {oldValues.DireccionEntrega}  -> A: {updatedValues.DireccionEntrega}");
                if (oldValues.Email != updatedValues.Email)
                    sb.AppendLine($"Email De: {oldValues.Email}  -> A: {updatedValues.Email}");
                //if (oldValues.Zona != updatedValues.Zona)
                //    {
                //    var azona = await (_context.ZONA.FirstAsync(z => z.ZON_CODIGO == oldValues.Zona));
                //    var nzona = await (_context.ZONA.FirstAsync(z => z.ZON_CODIGO == updatedValues.Zona));
                //    sb.AppendLine($"Email: {azona.ZON_NOMBRE} -> {nzona.ZON_NOMBRE}");
                //}
                if (oldValues.Parroquia != updatedValues.Parroquia)
                {
                    var aparroquia = await (_context.UBICACION.Where(z => z.UBI_CODIGO == oldValues.Parroquia)).ToListAsync();
                    var nparroquia = await (_context.UBICACION.Where(z => z.UBI_CODIGO == updatedValues.Parroquia)).ToListAsync();
                    sb.AppendLine($"Parroquia De: {aparroquia.FirstOrDefault()?.UBI_NOMBRE}  -> A: {nparroquia.FirstOrDefault()?.UBI_NOMBRE}");
                }
                if (oldValues.Provincia != updatedValues.Provincia)
                {
                    var aprovincia = await (_context.PROVINCIA.Where(z => z.ID_PROVINCIA_PK == oldValues.Provincia).ToListAsync());
                    var nprovincia = await (_context.PROVINCIA.Where(z => z.ID_PROVINCIA_PK == updatedValues.Provincia).ToListAsync());
                    sb.AppendLine($"Provincia De: {aprovincia.FirstOrDefault()?.NOMBRE_TX}  -> A: {nprovincia.FirstOrDefault()?.NOMBRE_TX}");
                }
                if (oldValues.Canton != updatedValues.Canton)
                {
                    var acanton = await (_context.CANTON.Where(z => z.ID_CANTON_PK == oldValues.Canton)).ToListAsync();
                    var ncanton = await (_context.CANTON.Where(z => z.ID_CANTON_PK == updatedValues.Canton)).ToListAsync();
                    sb.AppendLine($"Cantón De: {acanton.FirstOrDefault()?.NOMBRE_TX}  -> A: {ncanton.FirstOrDefault()?.NOMBRE_TX}");
                }

                if (oldValues.Establecimiento != updatedValues.Establecimiento)
                {
                    var aestable = await (_context.TESTABLECI.Where(z => z.TES_CODIGO == oldValues.Establecimiento)).ToListAsync();
                    var nestable = await (_context.TESTABLECI.Where(z => z.TES_CODIGO == updatedValues.Establecimiento)).ToListAsync();
                    sb.AppendLine($"Establecimiento De: {aestable.FirstOrDefault()?.TES_NOMBRE}  -> A: {nestable.FirstOrDefault()?.TES_NOMBRE}");
                }

                sb.AppendLine();
                sb.AppendLine("Por favor revise los cambios realizados.");

                string email = await _emailService.SendEmailAsync(P_CLI_NOT_MAILS, "ATENCIÓN ACTUALIZACIÓN DE CLIENTE" + " " + _singlecliente.CLI_NOMBRE, sb.ToString());

                response.Data = _singlecliente;
                response.Success = true;
                response.Message = "Cliente Actualizado " + email;
            }
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes. " + ex;

        }

        return response;
    }
    public async Task<decimal> ObtenerDeudaAsync(int empresa, decimal clienteCodigo)
    {
        var result = await _context.SaldoCarteraResults
             .FromSqlRaw("SELECT F_CXC_SALDO_CARTERA_PED_ST_NR(:empresa, :clienteCodigo) AS DEUDA FROM DUAL",
                         new OracleParameter("empresa", empresa),
                         new OracleParameter("clienteCodigo", clienteCodigo))
             .ToListAsync();

        return result.FirstOrDefault()?.DEUDA ?? 0;
    }

    public async Task<DateTime> ObtenerFechaSugeridaAsync(int empresa, decimal clienteCodigo, decimal agente)
    {
        var result = await _context.FechaSugResults
             .FromSqlRaw("SELECT F_VNT_FECHA_FACTURAR_DT(:empresa, :clienteCodigo, :cliAgente) AS FECHA_SUG FROM DUAL",
                         new OracleParameter("empresa", empresa),
                         new OracleParameter("clienteCodigo", clienteCodigo),
                         new OracleParameter("cliAgente", agente))
             .ToListAsync();

        return result.FirstOrDefault()?.FECHA_SUG ?? DateTime.Now;
    }


    public async Task<ServiceResponse<object>> UpdateRuteroxClienteAsync(AuxClienteApp cliente)
    {

        var response = new ServiceResponse<object>();
        try
        {
            if (cliente != null && cliente != null)
            {

                var _clienteExt_ = await (_context.CLIENTE_EXT.Where(ce => ce.CLI_CODIGO == cliente.CLI_CODIGO).ToListAsync());
                var _clienteExt = _clienteExt_.FirstOrDefault();

                if (_clienteExt != null)
                {
                    _clienteExt.CLI_LATITUD_NR = cliente.CLI_LATITUD_NR;
                    _clienteExt.CLI_LONGITUD_NR = cliente.CLI_LONGITUD_NR;
                    if (_context.Entry(_clienteExt).State == EntityState.Unchanged)
                    {
                        // Forzar el estado a modificado si es necesario
                        _context.Entry(_clienteExt).State = EntityState.Modified;
                    }

                    int ccomfacisave = await _context.SaveChangesAsync();
                    if (ccomfacisave != 0)
                    {
                        response.Success = true;
                        response.Message = "RUTERO REGISTRADO CORRECTAMENTE";
                        response.Data = cliente;
                        return response;
                    }
                }
                else
                {
                    response.Success = true;
                    response.Message = "NO SE ENCONTRO REGISTRO REPORTAR A T.I";
                    response.Data = cliente;
                    return response;
                }

            }
            response.Success = true;
            response.Message = "NO SE PUDO REGISTRAR EL CLIENTE";
            response.Data = null;
            return response;

        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes. " + ex;
            response.Data = null;
            return response;

        }
    }



    public async Task<ServiceResponse<object>> GetClientexCedRucAsync(string cedruc)
    {

        // Crea un objeto CultureInfo en español
        CultureInfo ci = new CultureInfo("es-ES");

        var response = new ServiceResponse<object>();

        try
        {
            var clientes = await (from cl in _context.CLIENTE
                                  join p in _context.POLITICA on cl.CLI_POLITICAS equals p.POL_CODIGO
                                  join ce in _context.CLIENTE_EXT on cl.CLI_CODIGO equals ce.CLI_CODIGO
                                  //join cd in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd.CDI_CLIENTE into cdGroup
                                  //from cd in cdGroup.DefaultIfEmpty()
                                  where cl.CLI_EMPRESA == p_empresa
                                        && cl.CLI_TIPO == p_cli_Tipo
                                        && cl.CLI_INACTIVO == p_cli_Inactivo
                                        && cl.CLI_BLOQUEO == p_cli_Bloqueo
                                        && cl.CLI_RUC_CEDULA == cedruc
                                  select new ClienteDto
                                  {
                                      CLI_EMPRESA = cl.CLI_EMPRESA,
                                      CLI_CODIGO = cl.CLI_CODIGO,
                                      CLI_NOMBRE = cl.CLI_NOMBRE,
                                      CLI_AGENTE = cl.CLI_AGENTE,
                                      CLI_ID = cl.CLI_ID,
                                      CLI_LISTAPRE = cl.CLI_LISTAPRE ?? 0,
                                      CLI_ILIMITADOF = cl.CLI_ILIMITADO ?? 0,
                                      CLI_ZONA = cl.CLI_ZONA ?? 0,
                                      CLI_TELEFONO1 = cl.CLI_TELEFONO1,
                                      CLI_POLITICAS = cl.CLI_POLITICAS,
                                      CLI_DIRECCION = cl.CLI_DIRECCION,
                                      CLI_DIR_ENTREGA = cl.CLI_DIR_ENTREGA,
                                      CLI_NOMBRECOM = cl.CLI_NOMBRECOM,
                                      CLI_MAIL = cl.CLI_MAIL,
                                      CLI_RUC_CEDULA = cl.CLI_RUC_CEDULA,
                                      ID_PROVINCIA_FK = ce.ID_PROVINCIA_FK ?? 0,
                                      ID_CANTON_FK = ce.ID_CANTON_FK ?? 0,
                                      CLI_ESTABLECIMIENTO = cl.CLI_ESTABLECIMIENTO,
                                      POL_PORC_DESC = p.POL_PORC_DESC,
                                      POL_PORC_FINANC = p.POL_PORC_FINANC,
                                      POL_PORC_PRO_PAGO = p.POL_PORC_PRO_PAGO,
                                      POL_PORC_PAG_CONTA = p.POL_PORC_PAG_CONTA,
                                      POL_LINEA_CREDITO = p.POL_LINEA_CREDITO,
                                      POL_DIAS_PLAZO = p.POL_DIAS_PLAZO,
                                      POL_NRO_PAGOS = p.POL_NRO_PAGOS,
                                      CLI_PARROQUIA = cl.CLI_PARROQUIA,
                                      CUPO = cl.CLI_CUPO

                                  })
                 .OrderBy(x => x.CLI_NOMBRE)
                 .ToListAsync();

            var cliente = clientes.FirstOrDefault();

            if (cliente == null)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "Cliente no encontrado.";
                return response;
            }

            response.Data = cliente;
            response.Success = true;
            response.Message = "Clientes encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }


    public async Task<ServiceResponse<object>> ValidaClientexCedRucAsync(string cedruc)
    {

        // Crea un objeto CultureInfo en español
        CultureInfo ci = new CultureInfo("es-ES");

        var response = new ServiceResponse<object>();

        try
        {
            var clientes = await (from cl in _context.CLIENTE
                                  //join p in _context.POLITICA on cl.CLI_POLITICAS equals p.POL_CODIGO
                                  //join ce in _context.CLIENTE_EXT on cl.CLI_CODIGO equals ce.CLI_CODIGO
                                  //join cd in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd.CDI_CLIENTE into cdGroup
                                  //from cd in cdGroup.DefaultIfEmpty()
                                  where cl.CLI_EMPRESA == p_empresa
                                        && cl.CLI_TIPO == p_cli_Tipo
                                        //&& cl.CLI_INACTIVO == p_cli_Inactivo
                                        //&& cl.CLI_BLOQUEO == p_cli_Bloqueo
                                        && cl.CLI_RUC_CEDULA == cedruc
                                  select new
                                  {
                                      NOMBRE_COMERCIAL = cl.CLI_NOMBRECOM,
                                      TELEFONO = cl.CLI_TELEFONO1,
                                      EMAIL = cl.CLI_MAIL,
                                      DIRECCION_CLIENTE = cl.CLI_DIRECCION,
                                      DIRECCION_ENTREGA = cl.CLI_DIR_ENTREGA,
                                      NOMBRE_CLIENTE = cl.CLI_NOMBRE

                                  })
                 .OrderBy(x => x.NOMBRE_COMERCIAL)
                 .ToListAsync();

            var cliente = clientes.FirstOrDefault();

            if (cliente == null)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "Cliente no encontrado.";
                return response;
            }

            response.Data = cliente;
            response.Success = true;
            response.Message = "Clientes encontrados exitosamente.";
            response.Status = 500; // Ya existe el cliente con esa cédula o ruc
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }


    public async Task<ServiceResponse<object>> ValidaClienteNuevoxCedRucAsync(string cedruc, string direntrega)
    {

        // Crea un objeto CultureInfo en español
        CultureInfo ci = new CultureInfo("es-ES");

        var response = new ServiceResponse<object>();

        try
        {
            var clientes = await (from cl in _context.CLIENTES_NUEVOS
                                  where cl.CI_RUC == cedruc && cl.DIRECCION_ENTREGA == direntrega
                                  select new Clientes_Nuevos
                                  {
                                      CI_RUC = cl.CI_RUC,
                                      NOMBRE_CLIENTE = cl.NOMBRE_CLIENTE,
                                      DIRECCION_ENTREGA = cl.DIRECCION_ENTREGA

                                  })
                                   .ToListAsync();
            ///////////////////////////////////////// SI NO HAY EN CLIENTES NUEVO DEBE VERIFICAR EN LA TABLA CLIENTE  ////////////////

            var cliente = clientes.FirstOrDefault();


            if (cliente == null)
            {
                var cliente_ = await (from cl in _context.CLIENTE
                                 where cl.CLI_RUC_CEDULA == cedruc && cl.CLI_DIR_ENTREGA == direntrega
                                 select new Clientes_Nuevos
                                 {
                                     CI_RUC = cl.CLI_RUC_CEDULA,
                                     NOMBRE_CLIENTE = cl.CLI_NOMBRECOM,
                                     DIRECCION_ENTREGA = cl.CLI_DIR_ENTREGA
                                 })
                                 .ToListAsync();
                cliente = cliente_.FirstOrDefault();
            }
            
            if (cliente == null)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "Cliente no encontrado.";
                return response;
            }

            response.Data = cliente;
            response.Success = true;
            response.Message = cliente.NOMBRE_CLIENTE + " ya se encuentra registrado como cliente, PUEDE CREARLO CAMBIANDO LA DIRECCIÓN DE ENTREGA";
            response.Status = 500; // Ya existe el cliente con esa cédula o ruc
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }


    public async Task<ServiceResponse<object>> GetClientesDespachosAsync(decimal agente)
    {



        var response = new ServiceResponse<object>();

        try
        {
            var clientes = await _context.REP_DET_ESTADO_DESPACHO_PED
                   .Where(rd => rd.AGE_CODIGO == agente)
                   .Select(rd => new
                   {
                       CLI_NOMBRE = rd.CLI_NOMBRE,
                       //rd.PEC,
                       //rd.CFAC_CCO_PEDIDO
                   })
                   .Distinct()
                   .ToListAsync();

            if (!clientes.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "Clientes no encontrados.";
                return response;
            }

            response.Data = clientes;
            response.Success = true;
            response.Message = "Clientes encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }


    public async Task<ServiceResponse<object>> GetPedidosDesxClienteAsync(AuxGeneral auxGeneral)
    {



        var response = new ServiceResponse<object>();

        try
        {
            var resultados = await _context.REP_DET_ESTADO_DESPACHO_PED
                             .Where(r => r.AGE_CODIGO == auxGeneral.AGE_CODIGO && r.CLI_NOMBRE.Contains(auxGeneral.AuxString))
                             .Select(r => new
                             {
                                 r.CRT_FECHA,
                                 r.EMP_NOMBRE,
                                 r.ZON_NOMBRE,
                                 r.AGE_NOMBRE,
                                 r.CLI_NOMBRE,
                                 r.FAC,
                                 r.PEC,
                                 r.ESTADO_DESPACHO,
                                 r.CFAC_CCO_PEDIDO
                             })
                             .ToListAsync();

            if (!resultados.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "Clientes no encontrados.";
                return response;
            }

            response.Data = resultados;
            response.Success = true;
            response.Message = "Clientes encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }


    public async Task<ServiceResponse<object>> GetClientesNuevos(decimal agente)
    {

        // Crea un objeto CultureInfo en español
        CultureInfo ci = new CultureInfo("es-ES");

        var response = new ServiceResponse<object>();

        try
        {
            var hoy = DateTime.Today;
            hoy.AddDays(10);
            var clientes = await (from cn in _context.CLIENTES_NUEVOS
                                     .Where(cn => ((cn.CREA_FECHA >= hoy)

                                    || (cn.CREA_FECHA != null && cn.CREA_FECHA < hoy && cn.ESTADO != "M")) && cn.ID_AGENTE_FK == agente)

                                  select new Clientes_Nuevos
                                  {
                                      ID_SECUENCIA_PK = cn.ID_SECUENCIA_PK,
                                      ID_EMPRESA_FK = cn.ID_EMPRESA_FK,
                                      ID_AGENTE_FK = cn.ID_AGENTE_FK,
                                      CI_RUC = cn.CI_RUC,
                                      NOMBRE_CLIENTE = cn.NOMBRE_CLIENTE,
                                      NOMBRE_COMERCIAL = cn.NOMBRE_COMERCIAL,
                                      TELEFONO = cn.TELEFONO,
                                      EMAIL = cn.EMAIL,
                                      ID_PROVINCIA_FK = cn.ID_PROVINCIA_FK,
                                      ID_CANTON_FK = cn.ID_CANTON_FK,
                                      ID_ZONA_FK = cn.ID_ZONA_FK,
                                      DIRECCION_CLIENTE = cn.DIRECCION_CLIENTE,
                                      ID_ESTABLECIMIENTO_FK = cn.ID_ESTABLECIMIENTO_FK,
                                      ID_LISTA_PRECIO_FK = cn.ID_LISTA_PRECIO_FK,
                                      ESTADO = cn.ESTADO,
                                      ID_PARROQUIA_FK = cn.ID_PARROQUIA_FK,
                                      DIRECCION_ENTREGA = cn.DIRECCION_ENTREGA,
                                      TIPO_IDENTIFICACION = cn.TIPO_IDENTIFICACION,
                                      CREA_FECHA = cn.CREA_FECHA,
                                  })
                 .OrderByDescending(x => x.CREA_FECHA)
                 .Distinct()
                 .ToListAsync();

            if (clientes == null)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "Clientes no encontrados.";
                return response;
            }

            response.Data = clientes;
            response.Success = true;
            response.Message = "Clientes encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }

    public async Task<ServiceResponse<object>> GetClienteBloqueoAsync(decimal Cod_cliente)
    {
       
        var response = new ServiceResponse<object>();
        try
        {
            var clientes = await (from cl in _context.CLIENTE
                                  where cl.CLI_CODIGO == Cod_cliente
                                  select new AuxGeneral
                                  {
                                      AuxInt = cl.CLI_BLOQUEO,
                                      AuxDecimal = cl.CLI_CODIGO
                                  }).ToListAsync();
            var cliente = clientes.FirstOrDefault();

            if (cliente != null)
            {
                if (cliente.AuxInt == 1)
                {
                    response.Data = cliente;
                    response.Success = true;
                    response.Message = "El cliente está bloqueado.";
                    response.Status = 1; // Indica que el cliente está bloqueado
                    return response;
                }
                else
                {
                    response.Data = cliente;
                    response.Success = true;
                    response.Message = "El cliente no está bloqueado.";
                    response.Status = 0;
                    return response;
                }
            }
            else
            {
                response.Data = null;
                response.Success = true;
                response.Message = "Cliente no encontrado.";
                return response;
            }

        }
        catch (Exception ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los cliente." + ex.ToString(); ;
            response.Data = null;
            response.Status = 0;
            return response;
        }

       
    }



    public async Task<ServiceResponse<object>> GetClientesNavidadxAgente(decimal agente)
    {
        GetParametros();
        if (P_NAVIDAD == 0)
        {             
            var responseNoNav = new ServiceResponse<object>();
            responseNoNav.Data = null;
            responseNoNav.Success = true;
            responseNoNav.Message = "No está activada la opción de Navidad.";
            return responseNoNav;
        }

        var response = new ServiceResponse<object>();




        var preciosPermitidosNav = new HashSet<decimal>();
        if (!string.IsNullOrEmpty(P_LISTA_PNAV))
        {
            preciosPermitidosNav = P_LISTA_PNAV.Split(';')
                                               .Select(s => decimal.TryParse(s.Trim(), out var dec) ? dec : (decimal?)null)
                                               .Where(d => d.HasValue)
                                               .Select(d => d.Value)
                                               .ToHashSet();
        }



        try
        {

            var query = from cl in _context.CLIENTE
                        join p in _context.POLITICA on cl.CLI_POLITICAS equals p.POL_CODIGO
                        join ce in _context.CLIENTE_EXT on cl.CLI_CODIGO equals ce.CLI_CODIGO
                        // LEFT JOIN con CLIENTE_DIA para evitar duplicados
                        join cd_join in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd_join.CDI_CLIENTE into cd_group
                        from cd in cd_group.DefaultIfEmpty()
                        where cl.CLI_EMPRESA == p_empresa
                              && cl.CLI_TIPO == p_cli_Tipo
                              && cl.CLI_INACTIVO == p_cli_Inactivo

                              //&& cl.CLI_BLOQUEO == p_cli_Bloqueo
                              && cl.CLI_AGENTE == agente

                            && (preciosPermitidosNav.Count == 0 || preciosPermitidosNav.Contains(cl.CLI_LISTAPRE ?? -1))
                        // Agrupamos por cliente para obtener resultados únicos
                        group new { cl, p, ce, cd } by new { cl.CLI_CODIGO, cl.CLI_NOMBRE, cl.CLI_AGENTE, cl.CLI_ID, cl.CLI_LISTAPRE, cl.CLI_ILIMITADO, cl.CLI_ZONA, cl.CLI_TELEFONO1, cl.CLI_POLITICAS, cl.CLI_DIRECCION, cl.CLI_DIR_ENTREGA, cl.CLI_NOMBRECOM, cl.CLI_MAIL, cl.CLI_RUC_CEDULA, ce.ID_PROVINCIA_FK, ce.ID_CANTON_FK, cl.CLI_ESTABLECIMIENTO, p.POL_PORC_DESC, p.POL_PORC_FINANC, p.POL_PORC_PRO_PAGO, p.POL_PORC_PAG_CONTA, p.POL_LINEA_CREDITO, p.POL_DIAS_PLAZO, p.POL_NRO_PAGOS, cl.CLI_PARROQUIA, cl.CLI_CUPO, cl.CLI_BLOQUEO, ce.CLI_LATITUD_NR, ce.CLI_LONGITUD_NR, ce.CLI_LATITUD1_NR, ce.CLI_LONGITUD1_NR } into g
                        select new ClienteDto
                        {
                            
                            CLI_EMPRESA = p_empresa,
                            CLI_CODIGO = g.Key.CLI_CODIGO,
                            CLI_NOMBRE = g.Key.CLI_NOMBRE,
                            CLI_AGENTE = g.Key.CLI_AGENTE,
                            CLI_ID = g.Key.CLI_ID,
                            CLI_LISTAPRE = g.Key.CLI_LISTAPRE ?? 0,
                            CLI_ILIMITADOF = g.Key.CLI_ILIMITADO ?? 0,
                            CLI_ZONA = g.Key.CLI_ZONA ?? 0,
                            CDI_DIA = string.Empty,
                            CLI_TELEFONO1 = g.Key.CLI_TELEFONO1,
                            CLI_POLITICAS = g.Key.CLI_POLITICAS,
                            CLI_DIRECCION = g.Key.CLI_DIRECCION,
                            CLI_DIR_ENTREGA = g.Key.CLI_DIR_ENTREGA,
                            CLI_NOMBRECOM = g.Key.CLI_NOMBRECOM,
                            CLI_MAIL = g.Key.CLI_MAIL,
                            CLI_RUC_CEDULA = g.Key.CLI_RUC_CEDULA,
                            ID_PROVINCIA_FK = g.Key.ID_PROVINCIA_FK ?? 0,
                            ID_CANTON_FK = g.Key.ID_CANTON_FK ?? 0,
                            CLI_ESTABLECIMIENTO = g.Key.CLI_ESTABLECIMIENTO,
                            POL_PORC_DESC = g.Key.POL_PORC_DESC,
                            POL_PORC_FINANC = g.Key.POL_PORC_FINANC,
                            POL_PORC_PRO_PAGO = g.Key.POL_PORC_PRO_PAGO,
                            POL_PORC_PAG_CONTA = g.Key.POL_PORC_PAG_CONTA,
                            POL_LINEA_CREDITO = g.Key.POL_LINEA_CREDITO,
                            POL_DIAS_PLAZO = g.Key.POL_DIAS_PLAZO,
                            POL_NRO_PAGOS = g.Key.POL_NRO_PAGOS,
                            CLI_PARROQUIA = g.Key.CLI_PARROQUIA,
                            CUPO = g.Key.CLI_CUPO,
                            CLI_BLOQUEO = g.Key.CLI_BLOQUEO ?? 0,
                            CLI_LATITUD_NR = g.Key.CLI_LATITUD_NR ?? 0M,
                            CLI_LONGITUD_NR = g.Key.CLI_LONGITUD_NR ?? 0M,
                            CLI_LATITUD1_NR = g.Key.CLI_LATITUD1_NR ?? 0M,
                            CLI_LONGITUD1_NR = g.Key.CLI_LONGITUD1_NR ?? 0M

                        };

            var clientes = await query.OrderBy(x => x.CLI_NOMBRE).ToListAsync();


            if (clientes == null || !clientes.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = FormatosTexto.DatosNoEncontrados;
            }
            else
            {
                foreach (var cliente in clientes)
                {
                    cliente.DEUDA = 0;
                    cliente.DISPONIBLE = 0;
                    cliente.FECHA_SUG = DateTime.Now;
                }

                response.Data = clientes;
                response.Success = true;
                response.Message = "Clientes encontrados exitosamente.";
            }
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            // Log the exception details (ex) here as needed
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }
}


