using PIGGISWS.Models;

namespace PIGGISWS.Interfaces;

public interface IFirebaseNotificationService
{

    Task<string> GetAccessTokenAsync();
    Task<ServiceResponse<object>> SendFcmMessageAsync(long not_codigo);
    Task<ServiceResponse<object>> SendALLFcmMessageAsync();

}
