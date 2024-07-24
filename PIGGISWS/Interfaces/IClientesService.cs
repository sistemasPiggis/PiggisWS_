using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;

namespace PIGGISWS.Interfaces;

public interface IClientesService
{
    Task<ServiceResponse<object>> GetClientesxAgente(int agente);
    Task<ServiceResponse<object>> CreateCliente(AuxClientesNuevos cliente);


}