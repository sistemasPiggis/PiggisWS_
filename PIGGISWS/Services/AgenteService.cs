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
    private readonly ILogger<PedidoService> _logger;
    public AgenteService(ApplicationDbContext context, ILogger<PedidoService> logger)
    {
        _context = context;
        _logger = logger;
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
            _logger.LogError(" --------------------- ERROR ------------------ GetAgentes() " + ex.ToString() );
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener los clientes.";
            _logger.LogError(" --------------------- ERROR ------------------ GetAgentes() " + ex.ToString());
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
            _logger.LogError(" --------------------- ERROR ------------------ GetAgente() " + ex.ToString() + age_codigo);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Ocurrió un error al obtener del agente.";
            _logger.LogError(" --------------------- ERROR ------------------ GetAgente() " + ex.ToString() + age_codigo);
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


    public async Task<string> GetUsuarioAsync(decimal agente)
    {
        string usuario = string.Empty;
        try
        {
            var usuarios = await _context.USUARIO.Where(w => w.USR_AGENTE == agente).ToListAsync(); 
            usuario = usuarios.Select(x => x.USR_ID).FirstOrDefault() ?? string.Empty;

            return usuario;
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(" --------------------- ERROR ------------------ GetAgente() " + ex.ToString() + agente);
            return ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(" --------------------- ERROR ------------------ GetAgente() " + ex.ToString() + agente);
            return ex.Message;
        }

    }


    public async Task<ServiceResponse<object>> GetCodigoAgentexMailAsync(string mail)
    {
        decimal codigo = 0;
        var response = new ServiceResponse<object>();
        string mailup = mail.ToUpper();
        string Nombre = string.Empty;
        try
        {
            var agentes = await _context.AGENTE
                .Where(a => a.AGE_MAIL.ToUpper() == mailup
            && a.AGE_INACTIVO == 0).ToListAsync();
            codigo = agentes.Select(c => c.AGE_CODIGO).FirstOrDefault();
            Nombre = agentes.Select(c => c.AGE_NOMBRE).FirstOrDefault() ?? "AGENTE NO ESTÁ CORRECTAMENTE CONFIGURADO";
            if (codigo == 0)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "AGENTE NO ENCONTRADO ";
                return response;
            }

            response.Data = codigo;
            response.Success = true;
            response.Message = "BIENVENIDO EXISTOS!!! AGENTE:" + " " + Nombre;
            return response;
        }
        catch (NotFoundException ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener los pedidos " + ex.ToString();
            _logger.LogError(" --------------------- ERROR ------------------ GetCodigoAgentexMailAsync() " + ex.ToString() + mail);
            return response;
        }

    }


    public async Task<decimal> GetCodigoAgentexClientesync(decimal cli_codigo)
    {
        
        try
        {
            var cliente = await _context.CLIENTE
                .Where(c => c.CLI_CODIGO == cli_codigo).ToListAsync();
            decimal agente = cliente.Select(c => c.CLI_AGENTE).FirstOrDefault() ?? 0;

            return agente;
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(" --------------------- ERROR ------------------ GetCodigoAgentexMailAsync() " + ex.ToString() + cli_codigo);
            return 0;
        }

    }
}
