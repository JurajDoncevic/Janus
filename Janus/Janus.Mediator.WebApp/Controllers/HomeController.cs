using Janus.Mediator.WebApp.Models;
using Janus.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Janus.Mediator.WebApp.Controllers;
public class HomeController : Controller
{
    private readonly Logging.ILogger<HomeController>? _logger;

    public HomeController(Logging.ILogger? logger = null)
    {
        _logger = logger?.ResolveLogger<HomeController>();
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
