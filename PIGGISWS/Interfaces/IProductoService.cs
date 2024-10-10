using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IProductoService
{

    Task<ServiceResponse<object>> GetProductosxAgente(int agente);
    Task<ServiceResponse<object>> GetTopProductosxAgente(int agente);
    Task<ServiceResponse<object>> GetDescuentosxProductoAsync(int cproducto, decimal lprecios, decimal ccliente);
}
