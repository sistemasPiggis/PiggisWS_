using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Interfaces;

public interface ICarteraService
{
    Task<ServiceResponse<object>> GetCarteraxFacturaAsync(Cartera cartera);
    Task<ServiceResponse<object>> GetCarteraxAgenteReporteAsync(int agente);
    Task<ServiceResponse<object>> GetCarteraxFacturaDiaAsync(Cartera cartera);

    Task<ServiceResponse<object>> CreateFacturaCarteraAsync(AuxCartera cartera);
    Task<ServiceResponse<object>> GetClientesNcsAsync(decimal agente);

    Task<ServiceResponse<object>> GetNcsxCodigoAsync(decimal codigo);
    Task<string?> ObtenerNumeroComprobanteAsync(int cco_empresa, decimal cco_codigo);

    Task<ServiceResponse<object>> GetDetalleNcsxCodigoAsync(decimal codigo);
    Task<ServiceResponse<object>> GetDevolicionxCodigoAsync(decimal codigo);

    Task<ServiceResponse<object>> GetDevProductosAproxCodigoAsync(decimal codigo);    

    Task<ServiceResponse<object>> GetDevProductosDenxCodigoAsync(decimal codigo);
    Task<ServiceResponse<object>> GetCarteraxFacDiaCliAsync(Cartera cartera, decimal cliente);
    Task<ServiceResponse<object>> GetCarteraXClienteAsync(decimal cliente);
    Task<ServiceResponse<object>> GetAnticiposXClienteAsync(decimal cliente);
    Task<ServiceResponse<object>> GetReportesxAgenteAsync(decimal agente);
    Task<ServiceResponse<object>> GetReportexNumeroAsync(Cartera cartera);

    Task<ServiceResponse<object>> CierreReporteAsync(AuxGeneral auxGeneral);

    Task<ServiceResponse<object>> GetCarteraCompletaAsync(decimal agente);
}
