using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IDevolucionesService
{
    Task<ServiceResponse<object>> GetProDevxClienteAgeAsync(Cliente cliente);
    Task<ServiceResponse<object>> GetProDevMotivoseAsync();
}
