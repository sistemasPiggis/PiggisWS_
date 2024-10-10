using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Interfaces;

public interface IPedidoService
{
    Task<ServiceResponse<object>> GetPedidosxClienteCorte(decimal cliente);
    Task<ServiceResponse<object>> GetPedidosDetalle(AuxPedido auxPedidos);
    Task<ServiceResponse<object>> CreatePedidoAsync(AuxNuevoPedido auxNuevoPedido);
}
