using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IListaPreciosService
{
    Task<ServiceResponse<object>> GetListasPreciosAsync(int agente);
}
