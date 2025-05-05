using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IMarcacionService
{
    Task<ServiceResponse<object>> CreateMarcacionAsync(Tmp_Marcacion_Agente marcacion_Agente);
    Task<ServiceResponse<object>> GetHoraAsync();
    Task<ServiceResponse<object>> GetMarcacionesxAgenteAsync(int agente);
}
