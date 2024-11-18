using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IRuteroService
{
    Task<ServiceResponse<object>> SetRuteroPedidoAsync(decimal cliente, int agente, DateTime fecha, decimal zona);
    Task<ServiceResponse<object>> ValidaHoraPedidoAsync(int agente, DateTime fecha);
    Task<ServiceResponse<object>> SetVisitaAsync(Rutero rutero);
}
