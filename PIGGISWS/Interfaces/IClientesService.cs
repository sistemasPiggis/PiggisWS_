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
    Task<ServiceResponse<object>> UpdateRuteroxClienteAsync(AuxClienteApp cliente);

    Task<ServiceResponse<object>> GetClientexCedRucAsync(string cedruc);
    Task<ServiceResponse<object>> ValidaClientexCedRucAsync(string cedruc);
    Task<ServiceResponse<object>> GetClientesDespachosAsync(decimal agente);
    Task<ServiceResponse<object>> GetPedidosDesxClienteAsync(AuxGeneral auxGeneral);
    Task<ServiceResponse<object>> GetClientesNuevos(decimal agente);
    Task<ServiceResponse<object>> GetClienteBloqueoAsync(decimal Cod_cliente);

    Task<ServiceResponse<object>> GetClientesNavidadxAgente(decimal agente);

    Task<List<decimal>> GetCodsClientesDiaxAgente(decimal agente);
}