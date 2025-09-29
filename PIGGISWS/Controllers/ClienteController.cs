using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Services;
using System.IdentityModel.Tokens.Jwt;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class ClienteController : ControllerBase
{

    private readonly IClientesService _clientesService;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    //ModelResponse model = new ModelResponse();
    //Provincia provincia = new Provincia();

    public ClienteController(IClientesService clientesService)
    {
        _clientesService = clientesService;
    }
    [Authorize]
    [HttpGet("{agente}")]

    public async Task<IActionResult> GetClientesxAgente(int agente)
    {



        var response = await _clientesService.GetClientesxAgente(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }
    [Authorize]
    [HttpGet("GetClientexCodigo/{cli_codigo}")]

    public async Task<IActionResult> GetClientexCodigo(int cli_codigo)
    {



        var response = await _clientesService.GetClientexCodigo(cli_codigo);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("CreateClienteAsync")]
    public async Task<IActionResult> CreateClienteAsync([FromBody] AuxClientesNuevos cliente)
    {
        var response = await _clientesService.CreateClienteAsync(cliente);

        if (response.Success)
        {
            if (response.Status == 500)
            {
                return Ok(response);
            }
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("EditClienteAsync")]
    public async Task<IActionResult> EditClienteAsync([FromBody] Cliente_ClienteExt cliente)
    {
        var response = await _clientesService.EditClienteAsync(cliente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("UpdateRuteroxClienteAsync")]
    public async Task<IActionResult> UpdateRuteroxClienteAsync([FromBody] AuxClienteApp cliente)
    {
        var response = await _clientesService.UpdateRuteroxClienteAsync(cliente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }
    [Authorize]
    [HttpPost("GetClientexCedRucAsync")]
    public async Task<IActionResult> GetClientexCedRucAsync([FromBody] string cliente)
    {
        var response = await _clientesService.GetClientexCedRucAsync(cliente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

    [Authorize]
    [HttpPost("ValidaClientexCedRucAsync")]
    public async Task<IActionResult> ValidaClientexCedRucAsync([FromBody] string cliente)
    {
        var response = await _clientesService.ValidaClientexCedRucAsync(cliente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetClientesDespachosAsync")]
    public async Task<IActionResult> GetClientesDespachosAsync([FromBody] decimal agente)
    {
        var response = await _clientesService.GetClientesDespachosAsync(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }



    [Authorize]
    [HttpPost("GetPedidosDesxClienteAsync")]
    public async Task<IActionResult> GetPedidosDesxClienteAsync([FromBody] AuxGeneral auxGeneral)
    {
        var response = await _clientesService.GetPedidosDesxClienteAsync(auxGeneral);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetClientesNuevos")]
    public async Task<IActionResult> GetClientesNuevos([FromBody] decimal agente)
    {
        var response = await _clientesService.GetClientesNuevos(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetClienteBloqueoAsync")]
    public async Task<IActionResult> GetClienteBloqueoAsync([FromBody] decimal Cod_Cliente)
    {
        var response = await _clientesService.GetClienteBloqueoAsync(Cod_Cliente);

        if (response.Success)
        {
        
            return Ok(response);
        }

        return BadRequest(response.Message);
    }



    [Authorize]
    [HttpPost("GetClientesNavidadxAgente")]
    public async Task<IActionResult> GetClientesNavidadxAgente([FromBody] decimal agente)
    {
        var response = await _clientesService.GetClientesNavidadxAgente(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }
}
