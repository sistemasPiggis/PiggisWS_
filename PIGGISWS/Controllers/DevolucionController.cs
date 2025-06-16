using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models.Vistas;
using PIGGISWS.Models;
using Microsoft.AspNetCore.Authorization;
using PIGGISWS.Services;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class DevolucionController : ControllerBase
{
    // GET: DevolucionesController
    private readonly IDevolucionesService _devolucionesService;
    private readonly ILogger<PedidoController> _logger;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    //ModelResponse model = new ModelResponse();

    public DevolucionController( IDevolucionesService devolucionesService, ILogger<PedidoController> logger)
    {
        _devolucionesService = devolucionesService;
        _logger = logger;
    }


    [Authorize]
    [HttpPost("GetProDevxClienteAgeAsync")]

    public async Task<IActionResult> GetCarteraXFacturaAsync([FromBody] Cliente cliente)
    {
        var response = await _devolucionesService.GetProDevxClienteAgeAsync(cliente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }



    [Authorize]
    [HttpGet("GetProDevMotivoseAsync")]

    public async Task<IActionResult> GetProDevMotivoseAsync()
    {
        var response = await _devolucionesService.GetProDevMotivoseAsync();

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }



    [Authorize]
    [HttpPost("CreateDevolucionAsync")]

    public async Task<IActionResult> CreateDevolucionAsync([FromBody] AuxDevolucion auxNuevaDevolucion)
    {


        if (auxNuevaDevolucion == null)
        {
            _logger.LogError("El cuerpo de la solicitud está vacío.");
            return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
        }

        _logger.LogInformation("Solicitud recibida para CreatePedidoAsync: {@auxNuevaDevolucion}", auxNuevaDevolucion);

        // Log detallado de cada propiedad del modelo
        if (auxNuevaDevolucion.Cabecera == null)
        {
            _logger.LogError("Cabecera está vacío.");
        }
        else
        {
            _logger.LogInformation("Ccomprobai: {@auxNuevaDevolucion.Cabecera}", auxNuevaDevolucion.Cabecera);
        }

        if (auxNuevaDevolucion.Ext == null)
        {
            _logger.LogError("ext está vacío.");
        }
        else
        {
            _logger.LogInformation("Ccomfaci: {@auxNuevaDevolucion.Ext}", auxNuevaDevolucion.Ext);
        }

        if (auxNuevaDevolucion.Detalle == null || !auxNuevaDevolucion.Detalle.Any())
        {
            _logger.LogError("DFacturai está vacío o no contiene elementos.");
        }
        else
        {
            _logger.LogInformation("DFacturai: {@auxNuevaDevolucion.Detalle}", auxNuevaDevolucion.Detalle);
        }

        var response = await _devolucionesService.CreateDevolucionAsync(auxNuevaDevolucion);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            _logger.LogInformation("Pedido creado exitosamente: {@Response}", response);
            return Ok(response);
        }

        _logger.LogError("Error al crear el pedido: {Message}", response.Message);
        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetProDevsxAgeAsync")]

    public async Task<IActionResult> GetProDevsxAgeAsync([FromBody] decimal agente)
    {
        var response = await _devolucionesService.GetProDevsxAgeAsync(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetSecuenciaDevAsync")]

    public async Task<IActionResult> GetSecuenciaDevAsync([FromBody] decimal agente)
    {
        var response = await _devolucionesService.GetSecuenciaDevAsync(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetDevsxAgeAsync")]

    public async Task<IActionResult> GetDevsxAgeAsync([FromBody] decimal agente)
    {
        var response = await _devolucionesService.GetDevsxAgeAsync(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetDevxCodigoAsync")]

    public async Task<IActionResult> GetDevxCodigoAsync([FromBody] AuxGeneral dev)
    {
        var response = await _devolucionesService.GetDevxCodigoAsync(dev);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

    [Authorize]
    [HttpPost("GetDevDetxCodigoAsync")]

    public async Task<IActionResult> GetDevDetxCodigoAsync([FromBody] AuxGeneral dev)
    {
        var response = await _devolucionesService.GetDevDetxCodigoAsync(dev);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetDevProdApCodigoAsync")]

    public async Task<IActionResult> GetDevProdApCodigoAsync([FromBody] AuxGeneral dev)
    {
        var response = await _devolucionesService.GetDevProdApCodigoAsync(dev);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetDevProDenCodigoAsync")]

    public async Task<IActionResult> GetDevProDenCodigoAsync([FromBody] AuxGeneral dev)
    {
        var response = await _devolucionesService.GetDevProDenCodigoAsync(dev);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }
}
