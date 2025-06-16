using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IAgenteService
{

    Task<ServiceResponse<List<Agente>>> GetAgente(int age_codigo);
    Task<ServiceResponse<object>>GetCercasAgente(int age_codigo, string age_dia);
    Task<ServiceResponse<object>> GetAgentes();
    Task<string> GetUsuarioAsync(decimal agente);
    Task<ServiceResponse<object>> GetCodigoAgentexMailAsync(string mail);
}