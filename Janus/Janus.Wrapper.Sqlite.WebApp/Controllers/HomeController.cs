using Janus.Wrapper.Sqlite.WebApp.Models;
using Janus.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Janus.Wrapper.Sqlite.WebApp.ViewModels;
using Janus.Wrapper.Sqlite;
using Janus.Wrapper.Sqlite.WebApp;

namespace Janus.Wrapper.WebApp.Controllers;
public class HomeController : Controller
{
    private readonly Logging.ILogger<HomeController>? _logger;
    private readonly SqliteWrapperManager _wrapperManager;
    private readonly WrapperOptions _wrapperOptions;
    private readonly IConfiguration _configuration;
    public HomeController(SqliteWrapperManager wrapperManager, SqliteWrapperOptions wrapperOptions, IConfiguration configuration, Logging.ILogger? logger = null)
    {
        _wrapperManager = wrapperManager;
        _wrapperOptions = wrapperOptions;
        _configuration = configuration;
        _logger = logger?.ResolveLogger<HomeController>();
    }

    public IActionResult Index()
    {
        var viewModel = new WrapperInfoViewModel()
        {
            NodeId = _wrapperOptions.NodeId,
            CommunicationFormat = _wrapperOptions.CommunicationFormat,
            ListenPort = _wrapperOptions.ListenPort,
            NetworkAdapterType = _wrapperOptions.NetworkAdapterType,
            PersistenceConnectionString = _wrapperOptions.PersistenceConnectionString,
            TimeoutMs = _wrapperOptions.TimeoutMs,
            SourceConnectionString = _wrapperOptions.SourceConnectionString,
            WebPort = _configuration.GetSection("WebConfiguration").Get<WebConfiguration>().Port,
            AllowsCommandExecution = _wrapperOptions.AllowsCommands
        };
        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
