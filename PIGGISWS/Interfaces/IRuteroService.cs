using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IRuteroService
{
    Task<ServiceResponse<object>> SetRuteroPedidoAsync(decimal cliente, decimal agente, DateTime fecha, decimal zona);
    Task<ServiceResponse<object>> ValidaHoraPedidoAsync(decimal agente, DateTime fecha);
    Task<ServiceResponse<object>> SetVisitaAsync(Rutero rutero);
}
