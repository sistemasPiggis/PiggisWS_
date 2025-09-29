using Azure;
using Microsoft.EntityFrameworkCore;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIGGISWS.Services;

public class MenuService: IMenuService
{
    private readonly ApplicationDbContext _context;
    private readonly IAgenteService _agenteService;
    ServiceResponse<object> respuesta = new ServiceResponse<object>();
    private readonly ILogger<PedidoService> _logger;
    List<Parametros_Movil> parametros = new List<Parametros_Movil>();
    int p_empresa;
    int p_car_siglafac;
    decimal CRT_NUMERO;
    int p_dias_anticipo;
    string usuarioAg;

    public MenuService(ApplicationDbContext context, IAgenteService agenteService, ILogger<PedidoService> logger)
    {
        _context = context;

        _agenteService = agenteService;
        usuarioAg = string.Empty;
        _logger = logger;
    }
    public async Task<ServiceResponse<List<Parametros_Movil>>> GetMenusMovilAsync()
    {
        var response = new ServiceResponse<List<Parametros_Movil>>();

        try
        {
            var menus = await _context.PARAMETROS_MOVIL
                .Where(c => c.SERVICIO == "MenuService" || c.SERVICIO == "GENERAL").ToListAsync();

            response.Data = menus;
            response.Success = true;
            response.Message = "Menus Moviles encontrados";
            return response;
        }
        catch (NotFoundException ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los pedidos " + ex.ToString();
            _logger.LogError(" --------------------- ERROR ------------------ GetMenusMovil() " + ex.ToString() );
            return response;
        }

    }
}
