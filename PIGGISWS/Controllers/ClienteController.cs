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


}
