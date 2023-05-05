using Janus.Mediator.WebApp.Models;
using Janus.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Janus.Mediator.WebApp.ViewModels;

namespace Janus.Mediator.WebApp.Controllers;
public class HomeController : Controller
{
    private readonly Logging.ILogger<HomeController>? _logger;
    private readonly MediatorManager _mediatorManager;
    private readonly MediatorOptions _mediatorOptions;
    private readonly IConfiguration _configuration;
    public HomeController(MediatorManager mediatorManager, MediatorOptions mediatorOptions, IConfiguration configuration, Logging.ILogger? logger = null)
    {
        _mediatorManager = mediatorManager;
        _mediatorOptions = mediatorOptions;
        _configuration = configuration;
        _logger = logger?.ResolveLogger<HomeController>();
    }

    public IActionResult Index()
    {
        var viewModel = new MediatorInfoViewModel()
        {
            NodeId = _mediatorOptions.NodeId,
            CommunicationFormat = _mediatorOptions.CommunicationFormat,
            ListenPort = _mediatorOptions.ListenPort,
            NetworkAdapterType = _mediatorOptions.NetworkAdapterType,
            PersistenceConnectionString = _mediatorOptions.PersistenceConnectionString,
            TimeoutMs = _mediatorOptions.TimeoutMs,
            WebPort = _configuration.GetSection("WebConfiguration").Get<WebConfiguration>().Port,
            EagerStartup = _mediatorOptions.EagerStartup
        };
        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
