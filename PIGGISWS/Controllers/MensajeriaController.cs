using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Interfaces;
using PIGGISWS.Models.Auxiliares;
using PIGGISWS.Models.DTOs;
using PIGGISWS.Services.Utils;
using static Google.Apis.Requests.BatchRequest;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("[controller]")]
public class MensajeriaController : ControllerBase
{
    private readonly IFirebaseNotificationService _firebaseService;

    public MensajeriaController(IFirebaseNotificationService firebaseService)
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



    [HttpPost("SendALLFcmMessageAsync")]
    public async Task<IActionResult> SendALLFcmMessageAsync()
    {

        var response = await _firebaseService.SendALLFcmMessageAsync();
        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }
        return BadRequest(response.Message);

    }

    [Authorize]
    [HttpPost("RegistrarAPPAsync")]
    public async Task<IActionResult> RegistrarAPPAsync([FromBody] Fcm_Token fcm_Token)
    {

        var response = await _firebaseService.RegistrarAPPAsync(fcm_Token);
        if (response.Success)
        {
            response.Status = Response.StatusCode;
            return Ok(response);
        }
        return BadRequest(response.Message);

    }
}
