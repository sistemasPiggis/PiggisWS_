using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Data;
using PIGGISWS.Models.Vistas;
using PIGGISWS.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace PIGGISWS.Controllers;

[ApiController]
[Route("[controller]")]
public class CentroController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    List<Vl_Nomina_Centro_Costo> lista_ncc = new List<Vl_Nomina_Centro_Costo>();

    public CentroController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Authorize]
    public async Task<string> getCentroPadre(int id_departamento)
    {
        try
        {

            lista_ncc = await (from c in _context.VL_NOMINA_CENTRO_COSTO.
                                Where(c => c.ID_DEPARTAMENTO == id_departamento)
                               select c)
                                .ToListAsync();

            if (lista_ncc != null)
            {
                model.Mensaje = "Transacción Generada Exitosamente";
                model.Data = lista_ncc;
                model.Status = Response.StatusCode;
                return (JsonConvert.SerializeObject(model));
            }
            else
            {
                model.Mensaje = "No se encontró la entidad con el código proporcionado";
                model.Data = "";
                model.Status = Response.StatusCode;
                return (JsonConvert.SerializeObject(model));
            }
        }
        catch (Exception ex)
        {
            model.Mensaje = "No se pudo Guardar Los Datos Ingresados" + ex.ToString();
            model.Data = lista_ncc;
            model.Status = Response.StatusCode;
            return (JsonConvert.SerializeObject(model));
        }
    }
}
