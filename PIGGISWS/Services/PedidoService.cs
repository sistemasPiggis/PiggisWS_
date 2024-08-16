using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using System.Numerics;

namespace PIGGISWS.Services;

public class PedidoService:IPedidoService
{

    private readonly ApplicationDbContext _context;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();

    int p_empresa = 0;
    int p_cli_Tipo = 0;
    //int p_cli_Bloqueo = 0;
    int p_cli_Inactivo = 0;
    //int p_cli_cupo = 0;
    //string p_cli_estado = "";
    public PedidoService(ApplicationDbContext context)
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



    public async Task<ServiceResponse<object>> GetPedidosxClienteCorte(decimal cliente)
    {

        var response = new ServiceResponse<object>();
        var siglapedidod = new List<int> { 90000855, 180001667 };
        var p_fecha_pedido = DateTime.Today.AddDays(-120);
        try
        {

    


            var pedidos = await( from cc in _context.CCOMPROBA 
                                 join df in _context.DFACTURA on cc.CCO_CODIGO equals df.DFAC_CFAC_COMPROBA
                                 join ct in _context.CTIPOCOM on cc.CCO_SIGLA equals ct.CTI_CODIGO
                                 where cc.CCO_EMPRESA == p_empresa
                                 && cc.CCO_ESTADO != 9
                                 && siglapedidod.Contains(cc.CCO_SIGLA)
                                 && cc.CCO_FECHA >= p_fecha_pedido
                                 && cc.CCO_CODCLIPRO == cliente
                                 group new { cc, df } by new { cc.CCO_NUMERO, cc.CCO_FECHA, ct.CTI_NOMBRE, cc.CCO_DETALLE, cc.CCO_CODIGO, cc.CCO_DIA, cc.CCO_PERIODO, cc.CCO_CIE_COMPROBA, cc.CCO_AGENTE, cc.CCO_MES } into g
                                 select new
                                 {
                                     CCO_NUMERO = g.Key.CCO_NUMERO,
                                     CCO_FECHA = g.Key.CCO_FECHA,
                                     CTI_NOMBRE = g.Key.CTI_NOMBRE,
                                     CCO_CODIGO = Convert.ToString( g.Key.CCO_CODIGO),
                                     DFAC_CANTIDAD = g.Sum(x => x.df.DFAC_CANTIDAD),
                                     DFAC_TOTAL = g.Sum(x => x.df.DFAC_TOTAL),
                                     CCO_DETALLE = g.Key.CCO_DETALLE,
                                     CCO_DIA = g.Key.CCO_DIA,
                                     CCO_PERIODO = g.Key.CCO_PERIODO,
                                     CCO_MES = g.Key.CCO_MES,
                                     CCO_CIE_COMPROBA = g.Key.CCO_CIE_COMPROBA,
                                     CCO_AGENTE = g.Key.CCO_AGENTE
                                 }
                                ).ToListAsync();
                                 







            if (pedidos == null || !pedidos.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = "pedidos no encontrados.";
                return response;
            }

            response.Data = pedidos;
            response.Success = true;
            response.Message = "pedidos encontrados exitosamente.";
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
            response.Message = "Ocurrió un error al obtener los pedidos";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }



    public async Task<ServiceResponse<object>> GetPedidosDetalle(AuxPedido auxPedidos)
    {

        var response = new ServiceResponse<object>();
        var siglapedidod = new List<int> { 90000855, 180001667 };
       

        try
        {
            
            var result = await (from cc in _context.CCOMPROBA
                                join df in _context.DFACTURA on cc.CCO_CODIGO equals df.DFAC_CFAC_COMPROBA
                                join pr in _context.PRODUCTO on df.DFAC_PRODUCTO equals pr.PRO_CODIGO
                                join ct in _context.CTIPOCOM on cc.CCO_SIGLA equals ct.CTI_CODIGO
                                where cc.CCO_EMPRESA == p_empresa
                                && cc.CCO_ESTADO != 9
                                && siglapedidod.Contains(cc.CCO_SIGLA)
                                && cc.CCO_NUMERO == auxPedidos.Cabecera.CCO_NUMERO
                                && cc.CCO_PERIODO == auxPedidos.Cabecera.CCO_PERIODO
                                && cc.CCO_DIA == auxPedidos.Cabecera.CCO_DIA
                                && cc.CCO_CIE_COMPROBA == auxPedidos.Cabecera.CCO_CIE_COMPROBA
                                && cc.CCO_MES == auxPedidos.Cabecera.CCO_MES
                                group new { cc, df } by new { cc.CCO_NUMERO, cc.CCO_FECHA, ct.CTI_NOMBRE, cc.CCO_DETALLE, cc.CCO_CODIGO, cc.CCO_DIA, cc.CCO_PERIODO, cc.CCO_CIE_COMPROBA, 
                                    cc.CCO_AGENTE, cc.CCO_MES, df.DFAC_CANTIDAD, df.DFAC_TOTAL,df.DFAC_SECUENCIA, pr.PRO_NOMBRE } into g
                                select new
                                {
                                    CCO_NUMERO = g.Key.CCO_NUMERO,
                                    CCO_FECHA = g.Key.CCO_FECHA,
                                    CTI_NOMBRE = g.Key.CTI_NOMBRE,
                                    DFAC_CANTIDAD = g.Key.DFAC_CANTIDAD,
                                    CCO_DETALLE = g.Key.CCO_DETALLE,
                                    DFAC_TOTAL = g.Key.DFAC_TOTAL,
                                    DFAC_SECUENCIA = g.Key.DFAC_SECUENCIA,
                                    PRO_NOMBRE = g.Key.PRO_NOMBRE,
                                    

                                }
                    ).OrderBy(d=> d.DFAC_SECUENCIA).ToListAsync();

            if (result == null || !result.Any())
            {

                response.Data = null;
                response.Success = true;
                response.Message = "pedidos no encontrados.";
                return response;
            }

            var cabecera = new AuxCComproba
            {
                CCO_NUMERO = result.First().CCO_NUMERO,
                CCO_FECHA = result.First().CCO_FECHA,
                CCO_DETALLE = result.First().CCO_DETALLE,
                DFAC_CANTIDADT = result.Sum(x => x.DFAC_CANTIDAD),
                DFAC_TOTALT = result.Sum(x => x.DFAC_TOTAL),

                CCO_EMPRESA = p_empresa
            };

            var detalles = result.Select(r => new AuxDFactura
            {
                DFAC_CANTIDAD = r.DFAC_CANTIDAD,
                DFAC_TOTAL = r.DFAC_TOTAL,
                PRO_NOMBRE = r.PRO_NOMBRE,
                DFAC_SECUENCIA = r.DFAC_SECUENCIA,
               
            }).ToList();

            var pedido = new AuxPedido
            {
                Cabecera = cabecera,
                Detalles = detalles
            };
            

            response.Data = pedido;
            response.Success = true;
            response.Message = "pedidos encontrados exitosamente.";
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
