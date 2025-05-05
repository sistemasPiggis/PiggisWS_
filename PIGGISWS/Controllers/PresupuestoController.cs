using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models.Vistas;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class PresupuestoController :  ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPresupuestoService _presupuestoService;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    List<Rep_Cartera_Vencida> lista_rcv = new List<Rep_Cartera_Vencida>();

    public PresupuestoController(ApplicationDbContext context, IPresupuestoService presupuestoService)
    {
        _context = context;
       _presupuestoService = presupuestoService;
    }


    [Authorize]
    [HttpPost("GetPresAgePeriodoAsync/{presupuesto}")]

    public async Task<IActionResult> GetPresAgePeriodoAsync([FromBody] AuxPresupuesto presupuesto)
    {
        var response = await _presupuestoService.GetPresAgePeriodoAsync(presupuesto);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


    [Authorize]
    [HttpPost("GetPreXCanalPeAsync/{presupuesto}")]

    public async Task<IActionResult> GetPreXCanalPeAsync([FromBody] AuxPresupuesto presupuesto)
    {
        var response = await _presupuestoService.GetPreXCanalPeAsync(presupuesto);

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }

}
