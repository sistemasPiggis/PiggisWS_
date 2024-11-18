using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Models.Vistas;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class CarteraController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICarteraService _carteraService;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    List<Rep_Cartera_Vencida> lista_rcv = new List<Rep_Cartera_Vencida>();

    public CarteraController(ApplicationDbContext context, ICarteraService carteraService)
    {
        _context = context;
        _carteraService = carteraService;
    }
 

    [Authorize]
    [HttpPost("GetCarteraXFacturaAsync")]

    public async Task<IActionResult> GetCarteraXFacturaAsync([FromBody] Cartera cartera)
    {
        var response = await _carteraService.GetCarteraxFacturaAsync(cartera);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }



    [Authorize]
    [HttpPost("GetCarteraxAgenteReporteAsync")]

    public async Task<IActionResult> GetCarteraxAgenteReporteAsync([FromBody] int agente)
    {
        var response = await _carteraService.GetCarteraxAgenteReporteAsync(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }



    [Authorize]
    [HttpPost("GetCarteraxFacturaDiaAsync")]

    public async Task<IActionResult> GetCarteraxFacturaDiaAsync([FromBody] Cartera cartera)
    {
        var response = await _carteraService.GetCarteraxFacturaDiaAsync(cartera);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetCarteraxFacDiaCliAsync")]

    public async Task<IActionResult> GetCarteraxFacDiaCliAsync([FromBody] Cartera cartera, decimal cliente)
    {
        var response = await _carteraService.GetCarteraxFacDiaCliAsync(cartera, cliente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }



    [Authorize]
    [HttpPost("CreateFacturaCarteraAsync")]

    public async Task<IActionResult> CreateFacturaCarteraAsync([FromBody] AuxCartera cartera)
    {
        var response = await _carteraService.CreateFacturaCarteraAsync(cartera);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpGet("GetClientesNcsAsync/{agente}")]

    public async Task<IActionResult> GetClientesNcsAsync(decimal agente)
    {
        var response = await _carteraService.GetClientesNcsAsync(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

    [Authorize]
    [HttpGet("GetNcsxCodigoAsync/{codigo}")] /// tra e las ncs por codigo de referencia 

    public async Task<IActionResult> GetNcsxCodigoAsync( decimal codigo)
    {
        var response = await _carteraService.GetNcsxCodigoAsync(codigo);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

    [Authorize]
    [HttpGet("GetDetalleNcsxCodigoAsync/{codigo}")]

    public async Task<IActionResult> GetDetalleNcsxCodigoAsync(decimal codigo)
    {
        var response = await _carteraService.GetDetalleNcsxCodigoAsync(codigo);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpGet("GetDevolicionxCodigoAsync/{codigo}")]

    public async Task<IActionResult> GetDevolicionxCodigoAsync(decimal codigo)
    {
        var response = await _carteraService.GetDevolicionxCodigoAsync(codigo);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }



    [Authorize]
    [HttpGet("GetDevProductosAproxCodigoAsync/{codigo}")]

    public async Task<IActionResult> GetDevProductosAproxCodigoAsync(decimal codigo)
    {
        var response = await _carteraService.GetDevProductosAproxCodigoAsync(codigo);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpGet("GetDevProductosDenxCodigoAsync/{codigo}")]

    public async Task<IActionResult> GetDevProductosDenxCodigoAsync(decimal codigo)
    {
        var response = await _carteraService.GetDevProductosDenxCodigoAsync(codigo);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


}
