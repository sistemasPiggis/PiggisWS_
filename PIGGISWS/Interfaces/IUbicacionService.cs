using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IUbicacionService
{
    Task<ServiceResponse<object>> GetProvinciasxAgente(int agente);
    Task<ServiceResponse<object>> GetCiudadesxProvincia(List<Provincia> cantones);
    Task<ServiceResponse<object>> GetParroquiasxCanton(List<Canton> cantones);
    Task<ServiceResponse<object>> GetZonasxAgenteAsync(int agente);
    Task<ServiceResponse<object>> GetEstablecimientos();
    Task<ServiceResponse<object>> GetUbicacionxAgenteAsync(int agente); 
}
