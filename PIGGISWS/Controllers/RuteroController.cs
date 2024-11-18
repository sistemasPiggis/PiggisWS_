using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Services;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class RuteroController : Controller
{
    private readonly IRuteroService _ruteroService;

    public RuteroController(IRuteroService ruteroService)
    {
        _ruteroService = ruteroService;
    }


    [Authorize]
    [HttpPost("SetRuteroPedidoAsync")]

    public async Task<IActionResult> SetRuteroPedidoAsync(decimal cliente, int agente, DateTime fecha, decimal zona)
    {



        var response = await _ruteroService.SetRuteroPedidoAsync(cliente, agente, fecha, zona);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("SetVisitaAsync")]

    public async Task<IActionResult> SetVisitaAsync(Rutero rutero)
    {

        var response = await _ruteroService.SetVisitaAsync(rutero);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


}
