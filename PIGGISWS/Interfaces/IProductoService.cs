using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IProductoService
{

    Task<ServiceResponse<object>> GetProductosxAgente(int agente);
}
