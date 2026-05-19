using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using PIGGISWS.Interfaces;
using PIGGISWS.Models;
using PIGGISWS.Services;
using System.Diagnostics;

namespace PIGGISWS.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly IUserGroupService _userGroupService;
    private readonly GraphServiceClient _graphServiceClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, GraphServiceClient graphServiceClient, IUserGroupService userGroupService)
    {
        _logger = logger;
            _graphServiceClient = graphServiceClient;
        _userGroupService = userGroupService;
    }

    [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
    public async Task<IActionResult> Index()
    {
var user = await _graphServiceClient.Me.Request().GetAsync();
ViewData["GraphApiResult"] = user.DisplayName;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
