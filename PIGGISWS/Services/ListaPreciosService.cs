using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Services.Utils;

namespace PIGGISWS.Services;

public class ListaPreciosService: IListaPreciosService
{
    private readonly ApplicationDbContext _context;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();

    int p_empresa = 0;
    //int p_cli_Tipo = 0;
    //int p_cli_Bloqueo = 0;
    int P_LDP_INACTIVO = 0;
    //int p_cli_cupo = 0;
    //string p_cli_estado = "";
    public ListaPreciosService(ApplicationDbContext context)
    {
        _context = context;
        GetParametros();
    }
    public void GetParametros()
    {
        parametros = _context.PARAMETROS_MOVIL.Where(p => p.SERVICIO == nameof(ListaPreciosService) || p.SERVICIO == "GENERAL").ToList();
        p_empresa = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 3)?.VALOR ?? "0");
        P_LDP_INACTIVO = Convert.ToInt32(parametros.FirstOrDefault(p => p.CODIGO == 14)?.VALOR ?? "0");
    }



    public async Task<ServiceResponse<object>> GetListasPreciosAsync(int agente)
    {
        var response = new ServiceResponse<object>();

        try
        {
            var listasp = await (from l in _context.LISTAPRE 
                                 join la in _context.PRECIO_AGENTE on l.LPR_CODIGO equals la.ID_PRECIO_FK
                                 where la.ID_AGENTE_FK == agente
                                 select l).ToListAsync();

            if (!listasp.Any())
            {
                response.Data = null;
                response.Success = true;
                response.Message = FormatosTexto.DatosNoEncontrados;
                return response;
            }

            response.Data = listasp;
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

 

}
