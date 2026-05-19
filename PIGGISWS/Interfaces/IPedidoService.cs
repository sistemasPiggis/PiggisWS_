using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Models.Vistas;

namespace PIGGISWS.Interfaces;

public interface IPedidoService
{
    Task<ServiceResponse<object>> GetPedidosxClienteCorte(decimal cliente);
    Task<ServiceResponse<object>> GetPedidosDetalle(AuxPedido auxPedidos);
    Task<ServiceResponse<object>> CreatePedidoAsync(AuxNuevoPedido auxNuevoPedido);
    Task<ServiceResponse<object>> GetPedidosDiaxAgente(decimal agente);
    Task<ServiceResponse<object>> GetPedidosxDiaAsync(PedidosDiaRequest request);
    Task<ServiceResponse<object>> GetFacsxClienteAsync(decimal request);
    Task<ServiceResponse<object>> GetFacDetalleAsync(Rep_Cantidades_Pedidosa request);
    Task<ServiceResponse<object>> GetEnvHoyAsync(decimal agente);
    Task<ServiceResponse<object>> CreatePedidoNavAsync(AuxNuevoPedidoNav auxNuevoPedidoNav);
    Task<ServiceResponse<object>> GetPedidosNavidadAsync(decimal agente);

    Task<ServiceResponse<object>> GetPedidoNavidadDAsync(AuxGeneral auxGeneral);
    Task<ServiceResponse<List<AuxGeneral>>> GetPedidosxRefs(List<AuxGeneral> auxGeneralList);
}
