using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PIGGISWS.Data;

using PIGGISWS.Models;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class CalidadController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Bodega bodega = new Bodega();
    DateTime today = DateTime.Today;

    public CalidadController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("getPanificacion")]
    [Authorize]
    public string getPanificacion()
    {
        try
        {
            

            var query = from p in _context.PLANIFICA_REVISION
                        join pd in _context.PLANIFICA_REVISION_DET on p.PLR_CODIGO equals pd.PLR_CODIGO
                        join pr in _context.PLANTILLA_REVISION on pd.PLN_CODIGO equals  pr.PLN_CODIGO
                        join e in _context.EMPLEADO on pd.PLD_EMP_ASIGNADO equals e.EMP_CODIGO
                        where pd.PLD_ESTADO ==1 && pd.PLD_FECHA.HasValue ///// quemado en codigo considerar implementar tabla de parametrizacion
                        && pd.PLD_FECHA.Value.Date  == today.Date 
                        select p;
                        

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
