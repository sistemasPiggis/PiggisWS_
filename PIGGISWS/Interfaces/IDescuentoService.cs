using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IDescuentoService
{
    Task<ServiceResponse<object>> GetDescuentoxAgenteAsync(int agente);
}
