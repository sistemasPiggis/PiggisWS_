using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Models;
using PIGGISWS.Models.Vistas;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class CarteraController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    List<Rep_Cartera_Vencida> lista_rcv = new List<Rep_Cartera_Vencida>();

    public CarteraController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Authorize]
    public async Task<string> getCartera(int Cod_Cliente)
    {
        try
        {

            lista_rcv = await (from c in _context.REP_CARTERA_VENCIDA.
                                Where(c => c.CLI_CODIGO == Cod_Cliente)
                                select c)
                                .ToListAsync();
                  
            if (lista_rcv != null)
            {                
                    model.Mensaje = "Transacción Generada Exitosamente";
                    model.Data = lista_rcv;
                    model.Status = Response.StatusCode;
                    return (JsonConvert.SerializeObject(model));
            }
            else
            {
                model.Mensaje = "No se encontró la entidad con el código proporcionado";
                model.Data = Cod_Cliente;
                model.Status = Response.StatusCode;
                return (JsonConvert.SerializeObject(model));
            }
        }
        catch (Exception ex)
        {
            model.Mensaje = "No se pudo Guardar Los Datos Ingresados" + ex.ToString();
            model.Data = Cod_Cliente;
            model.Status = Response.StatusCode;
            return (JsonConvert.SerializeObject(model));
        }

    }
}
