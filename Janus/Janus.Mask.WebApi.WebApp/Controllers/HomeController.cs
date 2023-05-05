using Janus.Mask.WebApi.WebApp.Commons;
using Janus.Mask.WebApi.WebApp.Models;
using Janus.Mask.WebApi.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Janus.Mask.WebApi.WebApp.Controllers;
public class HomeController : Controller
{
    private readonly Logging.ILogger<HomeController>? _logger;
    private readonly WebApiMaskManager _maskManager;
    private readonly WebApiMaskOptions _maskOptions;
    private readonly IConfiguration _configuration;
    public HomeController(WebApiMaskManager maskManager, WebApiMaskOptions maskOptions, IConfiguration configuration, Logging.ILogger? logger = null)
    {
        _maskManager = maskManager;
        _maskOptions = maskOptions;
        _configuration = configuration;
        _logger = logger?.ResolveLogger<HomeController>();
    }

    public IActionResult Index()
    {
        var viewModel = new MaskInfoViewModel()
        {
            NodeId = _maskOptions.NodeId,
            CommunicationFormat = _maskOptions.CommunicationFormat,
            ListenPort = _maskOptions.ListenPort,
            NetworkAdapterType = _maskOptions.NetworkAdapterType,
            PersistenceConnectionString = _maskOptions.PersistenceConnectionString,
            TimeoutMs = _maskOptions.TimeoutMs,
            WebApiPort = _maskOptions.WebApiOptions.ListenPort,
            IsSSLUsed = _maskOptions.WebApiOptions.UseSSL,
            WebApiSecurePort = _maskOptions.WebApiOptions.ListenPortSecure,
            WebPort = _configuration.GetSection("WebConfiguration").Get<WebConfiguration>().Port,
            IsInstanceRunning = _maskManager.IsInstanceRunning,
            OperationOutcome = TempData.ToOperationOutcomeViewModel(),
            EagerStartup = _maskOptions.EagerStartup
        };
        return View(viewModel);
    }

    [HttpPost]
    public IActionResult StartWebApiInstance()
    {
        var startingResult = _maskManager.StartWebApi();

        TempData["Constants.IsSuccess"] = startingResult.IsSuccess;
        TempData["Constants.Message"] = startingResult.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult StopWebApiInstance()
    {
        var stoppingResult = _maskManager.StopWebApi();

        TempData["Constants.IsSuccess"] = stoppingResult.IsSuccess;
        TempData["Constants.Message"] = stoppingResult.Message;

        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
