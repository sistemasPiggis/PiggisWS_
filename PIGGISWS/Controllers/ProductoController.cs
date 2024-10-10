using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Interfaces;
using PIGGISWS.Services;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class ProductoController : ControllerBase
{

    private readonly IProductoService _productoservice;

    public ProductoController(IProductoService productoervices)
    {
        _productoservice = productoervices;
    }

    [Authorize]
    [HttpGet("GetProductosxAgente/{agente}")]

    public async Task<IActionResult> GetProductosxAgente(int agente)
    {



        var response = await _productoservice.GetProductosxAgente(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

    [Authorize]
    [HttpGet("GetTopProductosxAgente/{agente}")]
    public async Task<IActionResult> GetTopProductosxAgente(int agente)
    {
        var response = await _productoservice.GetTopProductosxAgente(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

}
