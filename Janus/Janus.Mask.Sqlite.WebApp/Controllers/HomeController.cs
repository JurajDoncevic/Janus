using Janus.Mask.LiteDB.WebApp.Commons;
using Janus.Mask.LiteDB.WebApp.Models;
using Janus.Mask.LiteDB.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Janus.Mask.LiteDB.WebApp.Controllers;
public class HomeController : Controller
{
    private readonly Logging.ILogger<HomeController>? _logger;
    private readonly LiteDbMaskManager _maskManager;
    private readonly LiteDbMaskOptions _maskOptions;
    private readonly IConfiguration _configuration;
    public HomeController(LiteDbMaskManager maskManager, LiteDbMaskOptions maskOptions, IConfiguration configuration, Logging.ILogger? logger = null)
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
            WebPort = _configuration.GetSection("WebConfiguration").Get<WebConfiguration>().Port,
            MaterializationConnectionString = _maskOptions.MaterializationConnectionString,
            OperationOutcome = TempData.ToOperationOutcomeViewModel(),
            EagerStartup = _maskOptions.EagerStartup
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> MaterializeDatabase()
    {
        var startingResult = await _maskManager.MaterializeDatabase();

        TempData["Constants.IsSuccess"] = startingResult.IsSuccess;
        TempData["Constants.Message"] = startingResult.Message;

        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
