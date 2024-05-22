using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Models;
using Newtonsoft.Json;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class ProviciaController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Provincia provincia = new Provincia();
    public ProviciaController(ApplicationDbContext context)
    {
        _context = context;
    }
    // GET: Provicia
    [HttpGet]
    public string Index()
    {

        try
        {

            var query = _context.PROVINCIA
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

  