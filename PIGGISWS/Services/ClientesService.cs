using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;

namespace PIGGISWS.Services;

public class ClientesService : IClientesService
{
    private readonly ApplicationDbContext _context;
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
    public ClientesService(ApplicationDbContext context)
    {
        _context = context;
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
    }


    public async Task<ServiceResponse<object>> GetClientesxAgente(int agente)
    {

        var response = new ServiceResponse<object>();

        try
        {
            var clientes = await (from cl in _context.CLIENTE
                                  where cl.CLI_EMPRESA == p_empresa
                                     && cl.CLI_TIPO == p_cli_Tipo
                                     && cl.CLI_INACTIVO == p_cli_Inactivo
                                     && cl.CLI_BLOQUEO == p_cli_Bloqueo
                                     && cl.CLI_AGENTE == agente
                                  join p in _context.POLITICA on cl.CLI_POLITICAS equals p.POL_CODIGO
                                  join cd in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cd.CDI_CLIENTE into cdGroup
                                  from cd in cdGroup.DefaultIfEmpty()
                                  select new
                                  {
                                      cl.CLI_EMPRESA,
                                      cl.CLI_CODIGO,
                                      cl.CLI_NOMBRE,
                                      cl.CLI_AGENTE,
                                      CLI_LISTAPRE = cl.CLI_LISTAPRE ?? 0,
                                      CLI_ILIMITADOF = cl.CLI_ILIMITADO ?? 0,
                                      CLI_ZONA = cl.CLI_ZONA ?? 0,
                                      cd.CDI_DIA,
                                      cl.CLI_TELEFONO1,
                                      cl.CLI_POLITICAS,
                                      cl.CLI_DIRECCION,
                                      cl.CLI_MAIL,
                                      cl.CLI_RUC_CEDULA,
                                      cl.CLI_CIUDAD,
                                      p.POL_PORC_DESC,
                                      p.POL_PORC_FINANC,
                                      p.POL_PORC_PRO_PAGO,
                                      p.POL_PORC_PAG_CONTA,
                                      p.POL_LINEA_CREDITO,
                                      p.POL_DIAS_PLAZO,
                                      p.POL_NRO_PAGOS,
                                      //FECHA_SUG = DateTime.Now, averiguar 
                                      CUPO = 0,
                                      DEUDA = 0
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

    public async Task<ServiceResponse<object>> CreateCliente(AuxClientesNuevos clientes_Nuevos)
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


                // Suponiendo que 'clientes_Nuevos' contiene múltiples registros que necesitas agregar
                foreach (var nuevoCliente in clientes_Nuevos.Cliente_Dia_Gestion_Nuevo)
                {
                    var nuevoClienteDia = new Cliente_Dia_Gestion_Nuevo
                    {
                        ID_EMPRESA_FK = p_empresa,
                        TIPO_GESTION_TX = nuevoCliente.TIPO_GESTION_TX,
                        ID_CLIENTE_NUEVO_FK = cliente.CI_RUC, // Asegúrate de que esto tiene sentido en este contexto
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



    //public async Task<ServiceResponse<object>> UpdateCliente(Cliente cliente)
    //{
    //    var response = new ServiceResponse<object>();
    //    try
    //    {
    //       var cliente_ = new Cliente
    //        {
    //           CLI_NOMBRE = cliente.CLI_NOMBRE,
    //           CLI_TELEFONO1 = cliente.CLI_TELEFONO1,

    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        response.Message += ex.Message;
    //        response.Data = null;
    //        response.Success = false;
    //    }
    //}


}

