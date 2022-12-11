using Microsoft.AspNetCore.Mvc;
namespace Janus.Mediator.WebApp.Controllers;
public class RemotePointsController : Controller
{
    private readonly MediatorManager _mediatorManager;
    private Logging.ILogger<RemotePointsController>? _logger;

    public RemotePointsController(MediatorManager mediatorManager, Logging.ILogger? logger = null)
    {
        _mediatorManager = mediatorManager;
        _logger = logger?.ResolveLogger<RemotePointsController>();
    }

    public IActionResult Index()
    {
        var remotePoints = _mediatorManager.GetRegisteredRemotePoints();

        return View(remotePoints);
    }
}
