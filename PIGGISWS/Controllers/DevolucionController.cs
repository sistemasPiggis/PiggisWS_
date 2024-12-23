using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models.Vistas;
using PIGGISWS.Models;
using Microsoft.AspNetCore.Authorization;
using PIGGISWS.Services;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class DevolucionController : ControllerBase
{
    // GET: DevolucionesController
    private readonly ApplicationDbContext _context;
    private readonly IDevolucionesService _devolucionesService;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();

    public DevolucionController(ApplicationDbContext context, IDevolucionesService devolucionesService)
    {
        _context = context;
        _devolucionesService = devolucionesService;
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


}
