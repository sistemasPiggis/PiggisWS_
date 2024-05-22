using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Models;

namespace PIGGISWS.Controllers;

[ApiController]
[Route("[controller]")]
public class BancoController : ControllerBase
{

    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Banco_App banco_app = new Banco_App();

    public BancoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("getBanco")]
    [Authorize]
    public string getBanco()
    {
        try
        {

            var query = _context.BANCO_APP.Where(p => (p.BAN_INACTIVO ?? 0) == 0)
                .Select(p => new
                {
                    p.BAN_NOMBRE,
                    p.BAN_CODIGO,
                   
                }).OrderBy(P => P.BAN_NOMBRE)
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
