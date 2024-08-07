using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Interfaces;
using PIGGISWS.Models.Auxiliares;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class PedidoController : Controller
{
    // GET: PedidosController
    private readonly IPedidoService _pedidoservice;

    public PedidoController(IPedidoService pedidoservice)
    {
        _pedidoservice = pedidoservice;
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
}
