using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Models;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class NoticiasController : ControllerBase
{

    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Gen_Noticias gen_noticias = new Gen_Noticias();

    public NoticiasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("getBeneficios")]
    [Authorize]
    public string getBeneficios()
    {
        try
        {

            var query = _context.GEN_NOTICIAS
                .Where(n => n.NOT_ESTADO  == "0" && n.NOT_TIPO == 2)
                .Select(n => new
                {
                    n.NOT_CODIGO,
                    n.NOT_TITULO,
                    n.NOT_DESCRIPCION,
                    NOT_IMAGEN = "http://190.12.5.82:2156/staticfile/" + n.NOT_IMAGEN,
                    n.NOT_ORDEN
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
    [HttpGet("getAvisos")]
    [Authorize]
    public string getAvisos()
    {
        try
        {

            var query = _context.PROMOCION_CATALOGO.Where(p => (p.INACTIVO_NR ?? 0) == 0 && (p.APP_AGENTES ?? 0) == 1)
                .Select(p => new
                {
                    p.ID_PROMOCION_CATALOGO_PK,
                    p.ID_EMPRESA_FK,
                    p.DESCRIPCION_TX,
                    IMAGEN = "http://190.12.5.82:2156/staticfile/" + p.IMAGEN_TX,
                    p.FILENAME,
                    p.ORDEN_NR
                }).OrderBy(P => P.ORDEN_NR)
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
