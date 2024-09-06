using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Services.Utils;
using static Google.Apis.Requests.BatchRequest;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class MensajeriaController : ControllerBase
{
    private readonly FirebaseNotificationService _firebaseService;

    public MensajeriaController(FirebaseNotificationService firebaseService)
    {
            _firebaseService = firebaseService;
    }

    // GET: Mensajeria
    [HttpPost("SendFirebaseNotification")]
    public async Task<IActionResult> SendFirebaseNotification(int not_codigo)
    {
       
            var response = await _firebaseService.SendFcmMessageAsync(not_codigo);
            if (response.Success)
            {
                response.Status = Response.StatusCode;
                return Ok(response);
            }
        return BadRequest(response.Message);

    }
}
