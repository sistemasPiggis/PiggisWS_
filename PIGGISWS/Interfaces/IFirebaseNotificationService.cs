using PIGGISWS.Models;
using PIGGISWS.Models.DTOs;

namespace PIGGISWS.Interfaces;

public interface IFirebaseNotificationService
{

    Task<string> GetAccessTokenAsync();
    Task<ServiceResponse<object>> SendFcmMessageAsync(long not_codigo);
    Task<ServiceResponse<object>> SendALLFcmMessageAsync();
    Task<ServiceResponse<object>> RegistrarAPPAsync(Fcm_Token fcm_Token);
    Task<ServiceResponse<object>> NotAppAgentesAsync();
}
