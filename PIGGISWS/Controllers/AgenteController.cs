using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Services;

namespace PIGGISWS.Controllers;

[ApiController]
[Route("[controller]")]
public class AgenteController : ControllerBase
{
    private readonly IAgenteService _agenteService;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Provincia provincia = new Provincia();

    public AgenteController(IAgenteService agenteService)
    {
        _agenteService = agenteService;
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> getAgente(int age_codigo) /// get menu app ventas piggis
    {

        var response = await _agenteService.GetAgente(age_codigo);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }
        response.Status = Response.StatusCode;
        return BadRequest(response);
    }

    [Authorize]
    [HttpGet("GetCercasAgente")]
    public async Task<IActionResult> GetCercasAgente(int age_codigo, string age_dia) 
    {

        var response = await _agenteService.GetCercasAgente(age_codigo, age_dia);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }
        response.Status = Response.StatusCode;
        return BadRequest(response);
    }


}
