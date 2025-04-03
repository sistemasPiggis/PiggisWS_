using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class MarcacionController : ControllerBase
{
    private readonly IMarcacionService _marcacionService;
    private readonly ILogger<PedidoController> _logger;


    public MarcacionController(IMarcacionService marcacionService, ILogger<PedidoController> logger)
    {
        _marcacionService = marcacionService;
        _logger = logger;
    }


    [Authorize]
    [HttpPost("CreateMarcacionAsync")]

    public async Task<IActionResult> CreateMarcacionAsync([FromBody] Tmp_Marcacion_Agente marcacion_Agente)
    {


        if (marcacion_Agente == null)
        {
            _logger.LogError("El cuerpo de la solicitud está vacío.");
            return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
        }

        _logger.LogInformation("Solicitud recibida para CreateMarcacionAsync: {@marcacion_Agente}", marcacion_Agente);


        var response = await _marcacionService.CreateMarcacionAsync(marcacion_Agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            _logger.LogInformation("Marcación creada exitosamente: {@Response}", response);
            return Ok(response);
        }

        _logger.LogError("Error al crear el marcación: {Message}", response.Message);
        return BadRequest(response.Message);
    }




    [Authorize]
    [HttpPost("GetHoraAsync")]

    public async Task<IActionResult> GetHoraAsync()
    {



        var response = await _marcacionService.GetHoraAsync();

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            _logger.LogInformation("hora consultada: {@Response}", response);
            return Ok(response);
        }

        _logger.LogError("Error al consultar hora: {Message}", response.Message);
        return BadRequest(response.Message);
    }
}
