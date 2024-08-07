using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;

namespace PIGGISWS.Services;

public class ProductoService : IProductoService
{

    private readonly ApplicationDbContext _context;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    List<Producto> Productos = new List<Producto>();

    int p_empresa = 0;
    int p_cli_Tipo = 0;
    int p_cli_Bloqueo = 0;
    int p_cli_Inactivo = 0;
    int p_cli_cupo = 0;
    string p_cli_estado = "";
    public ProductoService(ApplicationDbContext context)
    {
        _context = context;
        GetParametros();
    }

    public void GetParametros()
    {
        parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == "ProductoService" || p.SERVICIO == "GENERAL").ToList();
        p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
        p_cli_Inactivo = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 6)?.VALOR ?? "0");
    }


    public async Task<ServiceResponse<object>> GetProductosxAgente(int agente)
    {

        var response = new ServiceResponse<object>();
        var fechaActual = DateTime.Today; //.AddMonths(-4);


        try
        {

            var listaprecios = await _context.CLIENTE
                             .Where(cl => cl.CLI_EMPRESA == p_empresa
                             && cl.CLI_AGENTE == agente
                             && (cl.CLI_INACTIVO ?? 0) == p_cli_Inactivo
                             && cl.CLI_LISTAPRE != null)
                            .Select(cl => cl.CLI_LISTAPRE ?? 0) // Cambio aquí: seleccione directamente el valor int
                            .Distinct()
                            .ToListAsync();



            var productos = await (from p in _context.PRODUCTO
                                   join um in _context.UMEDIDA on p.PRO_UNIDAD equals um.UMD_CODIGO
                                   join lp in _context.DLISTAPRE on p.PRO_CODIGO equals lp.DLP_PRODUCTO
                                   
                                   where p.PRO_EMPRESA == p_empresa
                                     && p.PRO_INACTIVO != 1
                                     && (p.PRO_CRITICO ?? 0) == 0
                                     && (p.PRO_INACTIVO ?? 0) == 0
                                     && lp.DLP_FECHA_INI <= fechaActual
                                     && listaprecios.Contains(lp.DLP_LISTAPRE)
                                     && (lp.DLP_INACTIVO ?? 0) == 0
                                     && (lp.DLP_FECHA_FIN == null || lp.DLP_FECHA_FIN >= fechaActual)
                                   select new
                                   {
                                       lp.DLP_LISTAPRE,
                                       p.PRO_NOMBRE, 
                                       p.PRO_CODIGO,
                                       um.UMD_ID, 
                                       p.PRO_ID, 
                                       lp.DLP_FECHA_FIN, 
                                       p.PRO_UNIDAD, 
                                       p.PRO_UNIDAD2, 
                                       p.PRO_IMPUESTO, 
                                       p.PRO_PROMOCION
                                   }).GroupBy(x => x.PRO_CODIGO)
                                    .Select(g => g.First())
                                    .ToListAsync();








            if (productos == null || !productos.Any())
            {
                throw new NotFoundException("No se encontraron clientes.");
            }

            response.Data = productos;
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
}
