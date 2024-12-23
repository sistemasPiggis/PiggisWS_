using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIGGISWS.Models.Firebase;
using PIGGISWS.Services.Utils;

namespace PIGGISWS.Controllers;
[ApiController]
[Route("api/[controller]")]
public class MarcacionesFBController : ControllerBase
{
    private readonly FireBaseService _firebaseService;

    public MarcacionesFBController(FireBaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Marcacion>>> GetMarcaciones()
    {
        var marcaciones = await _firebaseService.GetMarcacionesAsync();
        return Ok(marcaciones);
    }

    
}