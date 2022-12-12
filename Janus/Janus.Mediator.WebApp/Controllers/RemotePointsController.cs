using Janus.Communication.Remotes;
using Janus.Mediator.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        var remotePoints = _mediatorManager.GetRegisteredRemotePoints()
                                           .Map(rp => new RemotePointViewModel()
                                           {
                                               Address = rp.Address,
                                               NodeId = rp.NodeId,
                                               Port = rp.Port,
                                               RemotePointType = rp.RemotePointType
                                           });

        ViewBag.OperationOutcome = TempData["Outcome"] is not null ? JsonSerializer.Deserialize<OperationOutcomeViewModel>(TempData["Outcome"]!.ToString()) : null;
        return View(remotePoints);
    }
    [HttpPost]
    public async Task<IActionResult> RegisterRemotePoint([FromForm] string address, [FromForm] int port)
    {
        var undeterminedRemotePoint = new UndeterminedRemotePoint(address, port);
        var result = await _mediatorManager.RegisterRemotePoint(undeterminedRemotePoint);

        TempData["Outcome"] = JsonSerializer.Serialize(new OperationOutcomeViewModel()
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message
        });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> PingRemotePoint([FromForm] string address, [FromForm] int port)
    {
        var undeterminedRemotePoint = new UndeterminedRemotePoint(address, port);
        var result = await _mediatorManager.SendHello(undeterminedRemotePoint);

        TempData["Outcome"] = JsonSerializer.Serialize(new OperationOutcomeViewModel()
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message
        });

        return RedirectToAction(nameof(Index));
    }

}
