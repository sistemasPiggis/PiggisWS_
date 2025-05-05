using PIGGISWS.Models.DTOs;
using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IPresupuestoService
{
    Task<ServiceResponse<object>> GetPresAgePeriodoAsync(AuxPresupuesto presupuesto);
    Task<ServiceResponse<object>> GetPreXCanalPeAsync(AuxPresupuesto presupuesto);
}
