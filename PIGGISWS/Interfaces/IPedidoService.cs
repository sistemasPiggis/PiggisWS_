using PIGGISWS.Models;
using PIGGISWS.Models.Auxiliares;

namespace PIGGISWS.Interfaces;

public interface IPedidoService
{
    Task<ServiceResponse<object>> GetPedidosxClienteCorte(decimal cliente);
    Task<ServiceResponse<object>> GetPedidosDetalle(AuxPedido auxPedidos);
}
