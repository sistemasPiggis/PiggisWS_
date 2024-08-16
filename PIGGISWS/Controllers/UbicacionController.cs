using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Services;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class UbicacionController : ControllerBase
{

    private readonly IUbicacionService _ubicacionservice;

    public UbicacionController(IUbicacionService ubicacionService)
    {
        _ubicacionservice = ubicacionService;
    }


    [Authorize]
    [HttpGet("GetProvinciasxAgente/{agente}")]

    public async Task<IActionResult> GetProvinciasxAgente(int agente)
    {



        var response = await _ubicacionservice.GetProvinciasxAgente(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetCiudadesxProvincia")]

    public async Task<IActionResult> GetCiudadesxProvincia([FromBody] List<Provincia> provincia)
    {



        var response = await _ubicacionservice.GetCiudadesxProvincia(provincia);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

    [Authorize]
    [HttpPost("GetParroquiasxCanton")]

    public async Task<IActionResult> GetParroquiasxCanton([FromBody] List<Canton> cantones)
    {



        var response = await _ubicacionservice.GetParroquiasxCanton(cantones);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

    [Authorize]
    [HttpPost("GetZonasxCanton")]

    public async Task<IActionResult> GetZonasxCanton([FromBody] List<Canton> cantones)
    {



        var response = await _ubicacionservice.GetZonasxCanton(cantones);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpGet("GetEstablecimientosAsync")]

    public async Task<IActionResult> GetEstablecimientosAsync()
    {



        var response = await _ubicacionservice.GetEstablecimientos();

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpGet("GetUbicacionxAgenteAsync/{agente}")]

    public async Task<IActionResult> GetUbicacionxAgenteAsync(int agente)
    {

        var response = await _ubicacionservice.GetUbicacionxAgenteAsync(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

}
