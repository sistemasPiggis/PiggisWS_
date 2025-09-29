using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Services.Utils;
using System.Globalization;
using System.Linq;

namespace PIGGISWS.Services;

public class ProductoService : IProductoService
{

    private readonly ApplicationDbContext _context;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    List<Producto> Productos = new List<Producto>();

    int p_empresa = 0;
    //int p_cli_Tipo = 0;
    //int p_cli_Bloqueo = 0;
    int p_cli_Inactivo = 0;
    string p_nav_CPR_ID = string.Empty;
    decimal p_nav_GPR_CODIGO = 0;
    //int p_cli_cupo = 0;
    //string p_cli_estado = "";
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
        p_nav_CPR_ID = parametros.FirstOrDefault(p => p.CODIGO == 58)?.VALOR ?? "0";
        p_nav_GPR_CODIGO = Convert.ToDecimal(parametros.FirstOrDefault(p => p.CODIGO == 59)?.VALOR ?? "0");
    }


    public async Task<ServiceResponse<object>> GetProductosxAgente(int agente)
    {

        var response = new ServiceResponse<object>();
        var fechaActual = DateTime.Today;
        DateTime d = fechaActual.Date;

        try
        {

            var listaprecios = await _context.CLIENTE
                             .Where(cl => cl.CLI_EMPRESA == p_empresa
                             && cl.CLI_AGENTE == agente
                             && (cl.CLI_INACTIVO ?? 0) == p_cli_Inactivo
                             && cl.CLI_LISTAPRE != null)
                            .Select(cl => cl.CLI_LISTAPRE ?? 0) 
                            .Distinct()
                            .ToListAsync();



            var productos = await (from l in _context.LISTAPRE 
                                   join dl in _context.DLISTAPRE on l.LPR_CODIGO equals dl.DLP_LISTAPRE
                                   join p in _context.PRODUCTO on dl.DLP_PRODUCTO equals p.PRO_CODIGO
                                   join u in _context.UMEDIDA on p.PRO_UNIDAD equals u.UMD_CODIGO
                                   where p.PRO_EMPRESA == p_empresa
                                     && p.PRO_INACTIVO != 1
                                     && (p.PRO_CRITICO ?? 0) == 0
                                     && (p.PRO_INACTIVO ?? 0) == 0
                                     && dl.DLP_FECHA_INI <= fechaActual
                                     && (dl.DLP_FECHA_FIN == null || dl.DLP_FECHA_FIN >= fechaActual)
                                     && listaprecios.Contains(l.LPR_CODIGO)
                                     && (dl.DLP_INACTIVO ?? 0) == 0
                                     //&& p.PRO_CODIGO == 90002865

                                   select new
                                   {
                                       dl.DLP_LISTAPRE,
                                       p.PRO_NOMBRE,
                                       p.PRO_CODIGO,
                                       u.UMD_ID,
                                       p.PRO_ID,
                                       dl.DLP_FECHA_FIN,
                                       dl.DLP_FECHA_INI,
                                       p.PRO_UNIDAD,
                                       p.PRO_UNIDAD2,
                                       p.PRO_IMPUESTO,
                                       p.PRO_PROMOCION,
                                       DESTACADO = p.PRO_PROMOCION == 1,
                                       dl.DLP_PRECIO
                                       
                                       //lp.DLP_PRECIO2,
                                       //lp.DLP_DESCUENTO
                                   })
                                    .ToListAsync();








            if (productos == null || !productos.Any())
            {
                response.Data = productos;
                response.Success = true;
                response.Message = "No se Encontraron Productos";
            }

            response.Data = productos;
            response.Success = true;
            response.Message = "Productos encontrados exitosamente.";
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
            response.Message = "Ocurrió un error al obtener los Productos.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }

    public async Task<ServiceResponse<object>> GetTopProductosxAgente(int agente)
    {

        var response = new ServiceResponse<object>();
        var fechaActual = DateTime.Today;
        var p_dias_lapso = DateTime.Now.AddDays(-90);
        var siglas = new List<int> { 673 }; 
        //long clienteId = 750398940;
        //int estadoExcluido = 9;
        System.DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;

        // Crea un objeto CultureInfo en español
        CultureInfo ci = new CultureInfo("es-ES");

        // Obtiene el nombre del día de la semana en español
        string dayName = ci.DateTimeFormat.GetDayName(dayOfWeek);
        //var diasPermitidos = new[] { "VIERNES", "DOMINGO" };
        var diasPermitidos = new[] {  "DOMINGO" };
        string dayformateado = dayName.ToUpper();
        dayformateado = FormatosTexto.RemoveDiacritics(dayformateado);


        try
        {

            var productos =  (
              from cc in _context.CCOMPROBA
              join cd in _context.DFACTURA on cc.CCO_CODIGO equals cd.DFAC_CFAC_COMPROBA
              join p in _context.PRODUCTO on cd.DFAC_PRODUCTO equals p.PRO_CODIGO
              join ct in _context.CTIPOCOM on cc.CCO_SIGLA equals ct.CTI_CODIGO
              join cl in _context.CLIENTE on cc.CCO_CODCLIPRO equals cl.CLI_CODIGO
              join cld in _context.CLIENTE_DIA on cl.CLI_CODIGO equals cld.CDI_CLIENTE
              where cc.CCO_FECHA >= p_dias_lapso
                    && cc.CCO_SIGLA == 673
                    && cl.CLI_AGENTE == agente
                    && (
                                            diasPermitidos.Contains(dayformateado)
                                            || (cld.CDI_DIA != null && cld.CDI_DIA == dayformateado)
                                        )
              select new
              {
                  cd.DFAC_PRODUCTO,
                  cc.CCO_CODCLIPRO
              }
          )
          .AsEnumerable() 
          .GroupBy(g => g.CCO_CODCLIPRO) 
          .SelectMany(g => g
              .GroupBy(x => x.DFAC_PRODUCTO) 
              .Select(productGroup => new
              {
                  DFAC_PRODUCTO = productGroup.Key,
                  CCO_CODCLIPRO = g.Key,
                  Ranking = productGroup.Count()
              })
              .OrderByDescending(x => x.Ranking) 
              .Take(50)) // Tomamos los 50 primeros de cada cliente
          .ToList(); 




            if (productos == null || !productos.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "TOP PRODUCTOS NO ENCONTRADOS.";
                return response;
            }

            response.Data = productos;
            response.Success = true;
            response.Message = "Productos encontrados exitosamente.";
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



    public async Task<ServiceResponse<object>> GetDescuentosxProductoAsync(decimal cproducto, decimal lprecios, decimal ccliente)
    {
        var response = new ServiceResponse<object>();
        DateTime _fecha = DateTime.Now;
        DateTime fecha = _fecha.Date;
        try
        {
            var des = await (from l in _context.DLISTADSC
                             where l.DLD_PRODUCTO == cproducto && l.DLD_LISTAPRE == lprecios //&& l.DLD_CLIENTE == ccliente
                             && l.DLD_INACTIVO == 0 && l.DLD_FECHA_INI < fecha &&  (l.DLD_FECHA_FIN == null || l.DLD_FECHA_FIN >= fecha)
                             select l).ToListAsync();
            
            if (!des.Any())
            {
                response.Data = 0;
                response.Success = true;
                response.Message = FormatosTexto.DatosNoEncontrados;
                return response;
            }

            response.Data = des;
            response.Success = true;
            response.Message = FormatosTexto.DatosEncontrados;
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
            response.Message = "Ocurrió un error en:" + nameof(ListaPreciosService) + ex;

        }

        return response;



    }



    #region Pedidos Navidad


    public async Task<ServiceResponse<object>> GetProductosNavidad()
    {

        var response = new ServiceResponse<object>();

        try
        {

            var productos = await (from p in _context.PRODUCTO
                             join cp in _context.CLASIFPROD on p.PRO_CLASIFICACION equals cp.CPR_CODIGO
                             join gp in _context.GPRODUCTO on p.PRO_GPRODUCTO equals gp.GPR_CODIGO
                             where p.PRO_INACTIVO == 0
                                   && cp.CPR_ID == p_nav_CPR_ID
                                   && gp.GPR_CODIGO != p_nav_GPR_CODIGO
                             orderby p.PRO_NOMBRE ascending
                             select new
                             {
                                 PRO_NOMBRE = p.PRO_ID + " - " + p.PRO_NOMBRE,
                                 PRO_CODIGO = p.PRO_CODIGO
                             }).ToListAsync();


            if (productos == null || !productos.Any())
            {
                response.Data = productos;
                response.Success = true;
                response.Message = "No se Encontraron Productos Navideños";
            }

            response.Data = productos;
            response.Success = true;
            response.Message = "Productos Navideños Encontrados Exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los Productos.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }

    #endregion
}
