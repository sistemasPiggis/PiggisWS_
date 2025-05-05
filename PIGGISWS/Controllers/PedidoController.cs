using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Interfaces;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class PedidoController : Controller
{
    // GET: PedidosController
    private readonly IPedidoService _pedidoservice;
    private readonly ILogger<PedidoController> _logger;

    public PedidoController(IPedidoService pedidoservice, ILogger<PedidoController> logger)
    {
        _pedidoservice = pedidoservice;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("GetPedidosxClienteCorte/{cliente}")]

    public async Task<IActionResult> GetPedidosxClienteCorte(int cliente)
    {



        var response = await _pedidoservice.GetPedidosxClienteCorte(cliente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

    

    [Authorize]
    [HttpPost("GetPedidosDetalle")]

    public async Task<IActionResult> GetPedidosDetalle([FromBody] AuxPedido auxPedido)
    {



        var response = await _pedidoservice.GetPedidosDetalle(auxPedido);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }




    [Authorize]
    [HttpPost("CreatePedidoAsync")]

    public async Task<IActionResult> CreatePedidoAsync([FromBody] AuxNuevoPedido auxNuevoPedido)
    {


        if (auxNuevoPedido == null)
        {
            _logger.LogError("El cuerpo de la solicitud está vacío.");
            return BadRequest("El cuerpo de la solicitud no puede estar vacío.");
        }

        // Log detallado del contenido de auxNuevoPedido
        _logger.LogInformation("Solicitud recibida para CreatePedidoAsync: {@AuxNuevoPedido}", auxNuevoPedido);

        // Log detallado de cada propiedad del modelo
        if (auxNuevoPedido.Ccomprobai == null)
        {
            _logger.LogError("Ccomprobai está vacío.");
        }
        else
        {
            _logger.LogInformation("Ccomprobai: {@Ccomprobai}", auxNuevoPedido.Ccomprobai);
        }

        if (auxNuevoPedido.Ccomfaci == null)
        {
            _logger.LogError("Ccomfaci está vacío.");
        }
        else
        {
            _logger.LogInformation("Ccomfaci: {@Ccomfaci}", auxNuevoPedido.Ccomfaci);
        }

        if (auxNuevoPedido.DFacturai == null || !auxNuevoPedido.DFacturai.Any())
        {
            _logger.LogError("DFacturai está vacío o no contiene elementos.");
        }
        else
        {
            _logger.LogInformation("DFacturai: {@DFacturai}", auxNuevoPedido.DFacturai);
        }

        var response = await _pedidoservice.CreatePedidoAsync(auxNuevoPedido);

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
    [HttpPost("GetPedidosDiaxAgente")]

    public async Task<IActionResult> GetPedidosDiaxAgente([FromBody] decimal agente)
    {



        var response = await _pedidoservice.GetPedidosDiaxAgente(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetPedidosxDiaAsync")]

    public async Task<IActionResult> GetPedidosxDiaAsync([FromBody] PedidosDiaRequest request)
    {



        var response = await _pedidoservice.GetPedidosxDiaAsync(request);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

}
