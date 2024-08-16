using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Interfaces;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class ListaPreciosController : ControllerBase
{
    private readonly IListaPreciosService _listaPreciosService;

    public ListaPreciosController(IListaPreciosService listaPreciosService)
    {
        _listaPreciosService = listaPreciosService;
    }

    [Authorize]
    [HttpGet("GetListasPreciosAsync/{agente}")]

    public async Task<IActionResult> GetListasPreciosAsync(int agente)
    {

        var response = await _listaPreciosService.GetListasPreciosAsync(agente);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }
}
