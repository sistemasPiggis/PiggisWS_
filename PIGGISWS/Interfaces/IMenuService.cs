using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IMenuService
{

    Task<ServiceResponse<List<Parametros_Movil>>> GetMenusMovilAsync();
}
