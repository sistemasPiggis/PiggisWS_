using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models.Vistas;
using PIGGISWS.Models;
using System.CodeDom;
using Microsoft.AspNetCore.Authorization;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class DescuentoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IDescuentoService _decuentoService;
    // GET: Clienteprivate readonly ApplicationDbContext _context;


    public DescuentoController(ApplicationDbContext context, IDescuentoService descuentoService)
    {
        _context = context;
        _decuentoService = descuentoService;
    }


    [Authorize]
    [HttpPost("GetDescuentoxAgenteAsync/{agente}")] 

    public async Task<IActionResult> GetDescuentosxAgenteAsync([FromBody] int agente)
    {
        var response = await _decuentoService.GetDescuentoxAgenteAsync(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

}
