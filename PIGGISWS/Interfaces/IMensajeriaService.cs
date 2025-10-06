using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Interfaces;

public interface IMensajeriaService
{
    Task<ServiceResponse<object>> CreateMensajeNavAsync(AuxGeneral aux);
}
