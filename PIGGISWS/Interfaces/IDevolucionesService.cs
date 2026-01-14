using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Models.Vistas;

namespace PIGGISWS.Interfaces;

public interface IDevolucionesService
{
    Task<ServiceResponse<object>> GetProDevxClienteAgeAsync(Cliente cliente);
    Task<ServiceResponse<object>> GetProDevMotivoseAsync();

    Task<ServiceResponse<object>> CreateDevolucionAsync(AuxDevolucion auxDevolucion);
    Task<ServiceResponse<object>> GetProDevsxAgeAsync(decimal agente);
    Task<ServiceResponse<object>> GetSecuenciaDevAsync(decimal agente);
    Task<ServiceResponse<object>> GetDevsxAgeAsync(decimal agente);
    Task<ServiceResponse<object>> GetDevxCodigoAsync(AuxGeneral dev);

    Task<ServiceResponse<object>> GetDevDetxCodigoAsync(AuxGeneral dev);

    Task<ServiceResponse<object>> GetDevProdApCodigoAsync(AuxGeneral dev);

    Task<ServiceResponse<object>> GetDevProDenCodigoAsync(AuxGeneral dev);

    Task<ServiceResponse<object>> GetTopIdvsNoGestxClienteAsync(decimal agente);

    Task<ServiceResponse<object>> GetDevsNoGxAgeAsync(decimal agente);

    Task<ServiceResponse<VM_REPORTE_VENTAS2023>> GetMetaDevs(decimal agenteId);
}
