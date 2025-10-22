using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Services;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuservice;
    private readonly ILogger<PedidoController> _logger;
    private readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Menu_Movil menu = new Menu_Movil();
    public MenuController(ApplicationDbContext context, IMenuService menuService, ILogger<PedidoController> logger)
    {
        _context = context;
        _menuservice = menuService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public string Index()
    {
        try
        {
            var query = _context.MENU_MOVIL
        .Where(m => m.MNV_INACTIVO == 0 && m.MNV_REPORTA == null)
     .Select(m => new
     {
         Id = m.MNV_CODIGO,
         Nombre = m.MNV_NOMBRE,
         Submenu = _context.MENU_MOVIL
             .Where(s => s.MNV_REPORTA == m.MNV_CODIGO && s.MNV_INACTIVO == 0 )
             .Select(s => new
             {
                 Id = s.MNV_CODIGO,
                 Nombre = s.MNV_NOMBRE
             })
             .ToList()
     })
     .ToList();



            model.Status = Response.StatusCode;
            model.Mensaje = "Consulta Realizada Correctamente";
            model.Data = query;
            string resp = JsonConvert.SerializeObject(model);

            return resp;
        }
        catch (Exception ex)
        {
            model.Status = Response.StatusCode;
            model.Mensaje = ex.ToString();
            string respuesta = JsonConvert.SerializeObject(model);

            return respuesta;
        }
    }


    [Authorize]
    [HttpPost("GetMenusMovilAsync")]

    public async Task<IActionResult> GetMenusMovilAsync()
    {



        var response = await _menuservice.GetMenusMovilAsync();

        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }

        return BadRequest(response.Message);
    }


}
