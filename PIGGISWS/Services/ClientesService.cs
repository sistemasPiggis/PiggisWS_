using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Services.Utils;
using System.Globalization;
using System.Text;

namespace PIGGISWS.Services;

public class ClientesService : IClientesService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
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
    public ClientesService(ApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
       
        _emailService = emailService;
        GetParametros();
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
    }


    public async Task<ServiceResponse<object>> GetClientesxAgente(int agente)
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
            var clientes = await (from cl in _context.CLIENTE
                                  join p in _context.POLITICA on cl.CLI_POLITICAS equals p.POL_CODIGO
                                  join ce in _context.CLIENTE_EXT on cl.CLI_CODIGO equals ce.CLI_CODIGO
                                  join cd in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd.CDI_CLIENTE into cdGroup
                                  from cd in cdGroup.DefaultIfEmpty()
                                  where cl.CLI_EMPRESA == p_empresa
                                        && cl.CLI_TIPO == p_cli_Tipo
                                        && cl.CLI_INACTIVO == p_cli_Inactivo
                                        && cl.CLI_BLOQUEO == p_cli_Bloqueo
                                        && cl.CLI_AGENTE == agente
                                        && (cd.CDI_DIA != null && cd.CDI_DIA == dayformateado) 
                                  select new
                                  {
                                      cl.CLI_EMPRESA,
                                      cl.CLI_CODIGO,
                                      cl.CLI_NOMBRE,
                                      cl.CLI_AGENTE,
                                      cl.CLI_ID, 
                                      CLI_LISTAPRE = cl.CLI_LISTAPRE ?? 0,
                                      CLI_ILIMITADOF = cl.CLI_ILIMITADO ?? 0,
                                      CLI_ZONA = cl.CLI_ZONA ?? 0,
                                      CDI_DIA = cd != null ? cd.CDI_DIA : null,  
                                      cl.CLI_TELEFONO1,
                                      cl.CLI_POLITICAS,
                                      cl.CLI_DIRECCION,
                                      cl.CLI_DIR_ENTREGA, 
                                      cl.CLI_NOMBRECOM,
                                      cl.CLI_MAIL,
                                      cl.CLI_RUC_CEDULA,
                                      ce.ID_PROVINCIA_FK,
                                      ce.ID_CANTON_FK,
                                      cl.CLI_ESTABLECIMIENTO,
                                      p.POL_PORC_DESC,
                                      p.POL_PORC_FINANC,
                                      p.POL_PORC_PRO_PAGO,
                                      p.POL_PORC_PAG_CONTA,
                                      p.POL_LINEA_CREDITO,
                                      p.POL_DIAS_PLAZO,
                                      p.POL_NRO_PAGOS,
                                      cl.CLI_PARROQUIA,
                                      CUPO = 0,  // Asegúrate de que este valor se maneja según la lógica de negocio
                                      DEUDA = 0  // Ídem
                                  })
                 .OrderBy(x => x.CLI_NOMBRE)
                 .ToListAsync();


            if (clientes == null || !clientes.Any())
            {
                throw new NotFoundException("No se encontraron clientes.");
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
                var cliente = new Clientes_Nuevos
                {
                   ID_EMPRESA_FK = p_empresa,
                   ID_AGENTE_FK= clientes_Nuevos.Clientes_Nuevos.ID_AGENTE_FK,
                   CI_RUC = clientes_Nuevos.Clientes_Nuevos.CI_RUC,
                   NOMBRE_CLIENTE = clientes_Nuevos.Clientes_Nuevos.NOMBRE_CLIENTE, 
                   NOMBRE_COMERCIAL = clientes_Nuevos.Clientes_Nuevos.NOMBRE_COMERCIAL, 
                   TELEFONO = clientes_Nuevos.Clientes_Nuevos.TELEFONO,
                   EMAIL = clientes_Nuevos.Clientes_Nuevos.EMAIL,
                   ID_PROVINCIA_FK = clientes_Nuevos.Clientes_Nuevos.ID_PROVINCIA_FK,
                   ID_CANTON_FK = clientes_Nuevos.Clientes_Nuevos.ID_CANTON_FK,
                   ID_ZONA_FK= clientes_Nuevos.Clientes_Nuevos.ID_ZONA_FK,
                   DIRECCION_CLIENTE = clientes_Nuevos.Clientes_Nuevos.DIRECCION_CLIENTE, 
                   ID_ESTABLECIMIENTO_FK = clientes_Nuevos.Clientes_Nuevos.ID_ESTABLECIMIENTO_FK,
                   ID_LISTA_PRECIO_FK = clientes_Nuevos.Clientes_Nuevos.ID_LISTA_PRECIO_FK, 
                   ESTADO = p_cli_estado,
                   ID_PARROQUIA_FK = clientes_Nuevos.Clientes_Nuevos.ID_PARROQUIA_FK,
                   DIRECCION_ENTREGA = clientes_Nuevos.Clientes_Nuevos.DIRECCION_ENTREGA,
                   TIPO_IDENTIFICACION = tipo_identificacion,
                   LATITUD_NR = clientes_Nuevos.Clientes_Nuevos.LATITUD_NR,
                   LONGITUD_NR = clientes_Nuevos.Clientes_Nuevos.LONGITUD_NR
                   
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
                    var nuevoClienteDia = new Cliente_Dia_Gestion_Nuevo
                    {
                        ID_EMPRESA_FK = p_empresa,
                        TIPO_GESTION_TX = nuevoCliente.TIPO_GESTION_TX,
                        ID_CLIENTE_NUEVO_FK = cliente.CI_RUC, 
                        DIA_NR = nuevoCliente.DIA_NR,
                        INACTIVO_NR = nuevoCliente.INACTIVO_NR,
                        DIRECCION_CLIENTE = nuevoCliente.DIRECCION_CLIENTE,
                        DIRECCION_ENTREGA = nuevoCliente.DIRECCION_ENTREGA
                    };

                    listaClientesDia.Add(nuevoClienteDia);
                }

                _context.CLIENTE_DIA_GESTION_NUEVO.AddRange(listaClientesDia);
                _context.SaveChanges();


                response.Message = "Registro creado exitosamente";
                response.Data = cliente;
                response.Success = true;

            }
            catch (Exception ex)
            {
                response.Message += ex.Message;
                response.Data = null;
                response.Success = false;
            }
        }
        else
        {
            response.Message = "Cédula mal digitada";
            response.Data = null;
            response.Success = false;
        }
        return  response;
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

                                  select new
                                  {
                                      cl.CLI_EMPRESA,
                                      cl.CLI_CODIGO,
                                      cl.CLI_NOMBRE,
                                      cl.CLI_NOMBRECOM,
                                      cl.CLI_AGENTE,
                                      cl.CLI_ID,
                                      CLI_LISTAPRE = cl.CLI_LISTAPRE ?? 0,
                                      CLI_ILIMITADOF = cl.CLI_ILIMITADO ?? 0,
                                      CLI_ZONA = cl.CLI_ZONA ?? 0,
                                      ////CDI_DIA = cd != null ? cd.CDI_DIA : null,
                                      cl.CLI_TELEFONO1,
                                      cl.CLI_POLITICAS,
                                      cl.CLI_DIRECCION,
                                      cl.CLI_DIR_ENTREGA,
                                      cl.CLI_MAIL,
                                      cl.CLI_RUC_CEDULA,
                                      cl.CLI_ESTABLECIMIENTO,
                                      cl.CLI_PARROQUIA,
                                      ce.ID_PROVINCIA_FK,
                                      ce.ID_CANTON_FK,
                                      p.POL_PORC_DESC,
                                      p.POL_PORC_FINANC,
                                      p.POL_PORC_PRO_PAGO,
                                      p.POL_PORC_PAG_CONTA,
                                      p.POL_LINEA_CREDITO,
                                      p.POL_DIAS_PLAZO,
                                      p.POL_NRO_PAGOS,
                                      CUPO = 0,  // Asegúrate de que este valor se maneja según la lógica de negocio
                                      DEUDA = 0  // Ídem
                                  })
                 .OrderBy(x => x.CLI_NOMBRE)
                 .ToListAsync();

            var cliente = clientes.FirstOrDefault();

            if (cliente == null )
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
            // Log the exception details (ex) here as needed
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
            if (cliente != null)
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
                }

                var oldValues = new
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


                
                _singlecliente.CLI_DIRECCION = cliente.Cliente.CLI_DIRECCION;
                _singlecliente.CLI_DIR_ENTREGA = cliente.Cliente.CLI_DIR_ENTREGA;
                _singlecliente.CLI_MAIL = cliente.Cliente.CLI_MAIL;
                
                _singlecliente.CLI_PARROQUIA = cliente.Cliente.CLI_PARROQUIA;
                _clienteExt.ID_PROVINCIA_FK = cliente.Cliente_Ext.ID_PROVINCIA_FK;
                _clienteExt.ID_CANTON_FK = cliente.Cliente_Ext.ID_CANTON_FK;
                _singlecliente.CLI_ESTABLECIMIENTO = cliente.Cliente.CLI_ESTABLECIMIENTO;
                _context.Entry(_singlecliente).Property(x => x.CLI_DIRECCION).IsModified = true;
                _context.Entry(_singlecliente).Property(x => x.CLI_DIR_ENTREGA).IsModified = true;
                _context.Entry(_singlecliente).Property(x => x.CLI_MAIL).IsModified = true;
                _context.Entry(_singlecliente).Property(x=> x.CLI_PARROQUIA).IsModified = true;
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


}

