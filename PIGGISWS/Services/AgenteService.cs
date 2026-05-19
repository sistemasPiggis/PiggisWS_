using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using PIGGISWS.Data;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PIGGISWS.Services;

public class AgenteService: IAgenteService
{
    private readonly ApplicationDbContext _context;
    // GET: Clienteprivate readonly ApplicationDbContext _context;
    ModelResponse model = new ModelResponse();
    Provincia provincia = new Provincia();
    List<Map_Cerca_Agente> lista_mapas = new List<Map_Cerca_Agente>();
    private readonly ILogger<AgenteService> _logger;
    private readonly IUserGroupService _userGroupService;
    public AgenteService(ApplicationDbContext context, ILogger<AgenteService> logger, 
            IUserGroupService userGroupService)
    {
        _context = context;
        _logger = logger;
        _userGroupService = userGroupService;
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
            _logger.LogError(" --------------------- ERROR ------------------ GetUsuarioAsync() " + ex.ToString() + agente);
            return ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(" --------------------- ERROR ------------------ GetUsuarioAsync() " + ex.ToString() + agente);
            return ex.Message;
        }

    }


    public async Task<ServiceResponse<Agente>> GetCodigoAgentexMailAsync(string mail)
    {
        int codigo = 0;
        var response = new ServiceResponse<Agente>();
        string mailup = mail.ToUpper();
        string nombre = string.Empty;
        string grupo = string.Empty;
        try
        {
            var agentes = await _context.AGENTE
                .Where(a => a.AGE_MAIL.ToUpper() == mailup
            && a.AGE_INACTIVO == 0).ToListAsync();
            codigo = agentes.Select(c => c.AGE_CODIGO).FirstOrDefault();
            nombre = agentes.Select(c => c.AGE_NOMBRE).FirstOrDefault() ?? "AGENTE NO ESTÁ CORRECTAMENTE CONFIGURADO";
            if (codigo == 0)
            {
                response.Data = null;
                response.Success = true;
                response.Message = "AGENTE NO ENCONTRADO ";
                return response;
            }

            // 2. Obtener grupo del usuario desde AD
            try
            {
                grupo = await _userGroupService.GetUserRoleAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error obteniendo grupo de AD para {mail}: {ex.Message}");
                grupo = "SIN_ASIGNAR";
            }


            // 3. Retornar datos completos
            response.Data = new Agente
            {
                AGE_CODIGO = codigo,
                AGE_NOMBRE = nombre,
                Grupo = grupo
            };
            response.Success = true;
            response.Message = $"BIENVENIDO EXITOSO!!! AGENTE: {nombre} - GRUPO: {grupo}";
            return response;
        }
        catch (NotFoundException ex)
        {

            response.Success = false;
            response.Message = "Ocurrió un error al obtener el agente " + ex.ToString();
            _logger.LogError(" --------------------- ERROR ------------------ GetCodigoAgentexMailAsync() " + ex.ToString() + mail);
            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al procesar la solicitud";
            _logger.LogError($"ERROR GetCodigoAgentexMailAsync() {ex} - Mail: {mail}");
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
