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
using PIGGISWS.Services;
using System.IdentityModel.Tokens.Jwt;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class ClienteController : ControllerBase
{

    private readonly IClientesService _clientesService;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Provincia provincia = new Provincia();

    public ClienteController(IClientesService clientesService)
    {
        _clientesService = clientesService;
    }
    [Authorize]
    [HttpGet("{agente}")]

    public async Task<IActionResult> GetClientesxAgente(int agente)
    {
        //var authHeader = Request.Headers["Authorization"].ToString();
        //if (string.IsNullOrEmpty(authHeader))
        //{
        //    return Unauthorized("Missing Authorization Header");
        //}

        //var token = authHeader.Split(' ')[1];

        //var validationParameters = new TokenValidationParameters
        //{
        //    ValidateIssuer = true,
        //    ValidIssuer = "https://sts.windows.net/c64eb65f-f8db-40c6-b0bc-869c3321dc58/",
        //    ValidateAudience = true,
        //    ValidAudience = "70aa7a64-f86f-4e63-84fb-aa9ed4277bd9",
        //    ValidateLifetime = true,
        //    IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
        //    {
        //        // Retrieve the signing keys from Azure AD
        //        var client = new HttpClient();
        //        var response = client.GetAsync("https://login.microsoftonline.com/common/discovery/keys").Result;
        //        var keys = new JsonWebKeySet(response.Content.ReadAsStringAsync().Result);
        //        return keys.Keys;
        //    }
        //};

        //var handler = new JwtSecurityTokenHandler();
        //try
        //{
        //    var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);
        //    // Token is valid
        //}
        //catch (Exception ex)
        //{
        //    return Unauthorized($"Token validation failed: {ex.Message}");
        //}


        var response = await _clientesService.GetClientesxAgente(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost]

    public async Task<IActionResult> CreateCliente(AuxClientesNuevos cliente)
    {
        var response = await _clientesService.CreateCliente(cliente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


}
