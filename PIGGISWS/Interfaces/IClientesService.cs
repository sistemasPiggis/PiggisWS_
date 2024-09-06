using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Interfaces;

public interface IClientesService
{
    Task<ServiceResponse<object>> GetClientesxAgente(int agente);
    Task<ServiceResponse<object>> CreateClienteAsync(AuxClientesNuevos cliente);
    Task<ServiceResponse<object>> GetClientexCodigo(long cli_codigo);
    Task<ServiceResponse<object>> EditClienteAsync(Cliente_ClienteExt cliente);
}