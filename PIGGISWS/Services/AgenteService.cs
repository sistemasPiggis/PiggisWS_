using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using System.Drawing;

namespace PIGGISWS.Services;

public class AgenteService: IAgenteService
{
    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Provincia provincia = new Provincia();
    List<Map_Cerca_Agente> lista_mapas = new List<Map_Cerca_Agente>();
    public AgenteService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<ServiceResponse<object>> GetAgentes()
    {
        var response = new  ServiceResponse<object>();

        try
        {
            var agentes = await _context.AGENTE.Where(w => w.AGE_REPORTA == 45)
                                        .ToListAsync();

            if (agentes == null || !agentes.Any())
            {
                throw new NotFoundException("No se encontraron clientes.");
            }

            response.Data = agentes;
            response.Success = true;
            response.Message = "Clientes encontrados exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            // Log the exception details (ex) here as needed
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }



    public async Task<ServiceResponse<List<Agente>>> GetAgente(int age_codigo)
    {
        var response = new ServiceResponse<List<Agente>>();

        try
        {
            var agentes = await _context.AGENTE.Where(w => w.AGE_CODIGO == age_codigo && w.AGE_INACTIVO ==0).ToListAsync(); //  w.AGE_INACTIVO ==0 solo agentes activos



            if (agentes == null || !agentes.Any())
            {

                throw new NotFoundException("No se encontraron agentes.");
            }

            response.Data = agentes;
            response.Success = true;
            response.Message = "Agente consultado exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener del agente.";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }

    public async Task<ServiceResponse<object>> GetCercasAgente(int age_codigo, string age_dia)
    {
        var response = new ServiceResponse<object>();

        try
        {
            var cercas = await _context.MAP_CERCA_AGENTE.Where(w => w.AGE_CODIGO == age_codigo && w.DIA == age_dia ).ToListAsync(); //  w.AGE_INACTIVO ==0 solo agentes activos



            if (cercas == null || !cercas.Any())
            {

                throw new NotFoundException("No se encontraron Cercas.");
            }

            response.Data = cercas;
            response.Success = true;
            response.Message = "Cerca consultado exitosamente.";
        }
        catch (NotFoundException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener las cercas";
            throw new DatabaseException("Error de base de datos.", ex);
        }

        return response;
    }
}
