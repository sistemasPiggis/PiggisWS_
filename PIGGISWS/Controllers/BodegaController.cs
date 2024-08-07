using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Models;

namespace PIGGISWS.Controllers;

[ApiController]
[Route("[controller]")]
public class BodegaController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Bodega bodega = new Bodega();

    public BodegaController(ApplicationDbContext context)
    {
        _context = context;
    }
    [Authorize]
    [HttpGet("GetUsuarioBodega/{agente}")]
  
    public async Task<string> GetUsuarioBodega(int agente)
    {

        try
        {
   
            
            var query = await ( from u in _context.USUARIO
                        join us in _context.USRBOD on u.USR_CODIGO equals us.UBO_USUARIO
                        join b in _context.BODEGA on us.UBO_BODEGA equals b.BOD_CODIGO
                        join a in _context.AGENTE  on u.USR_AGENTE equals a.AGE_CODIGO
                        where a.AGE_CODIGO == agente
                        orderby us.UBO_DEFAULT descending
                        select new
                        {
                          b.BOD_EMPRESA,
                          us.UBO_BODEGA,
                          us.UBO_USUARIO,
                          b.BOD_NOMBRE,
                          b.BOD_CODIGO,
                          us.UBO_DEFAULT
                        }).ToListAsync();

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
