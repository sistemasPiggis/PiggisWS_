using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Models;

namespace PIGGISWS.Controllers;

[ApiController]
[Route("[controller]")]
public class AgenteController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Provincia provincia = new Provincia();

    public AgenteController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Authorize]
    public string getAgente(int age_codigo) /// get menu app ventas piggis
    {

        try
        {

            var query = _context.AGENTE.Where(a => a.AGE_CODIGO == age_codigo)
                .Select(a => new
                {
                    a.AGE_EMPRESA,
                    a.AGE_CODIGO,
                    a.AGE_NOMBRE,   
                    a.AGE_BODEGA, 
                    a.AGE_ALMACEN,
                    a.AGE_UBICACION,
                    a.AGE_REPORTA,
                    a.AGE_MAIL
                })
                .ToList();

            model.Status = Response.StatusCode;
            model.Mensaje = "Consulta Realizada Correctamente";
            model.Data = query;
            string resp = JsonConvert.SerializeObject(model);

            return resp;
        }
        catch (Exception ex)
        {
            model.Status = Response.StatusCode;
            model.Mensaje = ex.ToString();
            string respuesta = JsonConvert.SerializeObject(model);

            return respuesta;
        }

    }

    
}
