using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Interfaces;

public interface IDevolucionesService
{
    Task<ServiceResponse<object>> GetProDevxClienteAgeAsync(Cliente cliente);
    Task<ServiceResponse<object>> GetProDevMotivoseAsync();

    Task<ServiceResponse<object>> CreateDevolucionAsync(AuxDevolucion auxDevolucion);
}
