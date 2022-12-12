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

    public IActionResult Index(OperationOutcomeViewModel? operationOutcome = null)
    {
        var remotePoints = _mediatorManager.GetRegisteredRemotePoints()
                                           .Map(rp => new RemotePointViewModel()
                                           {
                                               Address = rp.Address,
                                               NodeId = rp.NodeId,
                                               Port = rp.Port,
                                               RemotePointType = rp.RemotePointType
                                           });

        var viewModel = new RemotePointsListViewModel()
        {
            RemotePoints = remotePoints.ToList(),
            OperationOutcome = operationOutcome
        };

        return View(viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> RegisterRemotePoint([FromForm] string address, [FromForm] int port)
    {
        var undeterminedRemotePoint = new UndeterminedRemotePoint(address, port);
        var result = await _mediatorManager.RegisterRemotePoint(undeterminedRemotePoint);

        var operationOutcome = new OperationOutcomeViewModel()
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message
        };

        return RedirectToAction(nameof(Index), operationOutcome);
    }

    [HttpPost]
    public async Task<IActionResult> PingRemotePoint([FromForm] string address, [FromForm] int port)
    {
        var undeterminedRemotePoint = new UndeterminedRemotePoint(address, port);
        var result = await _mediatorManager.SendHello(undeterminedRemotePoint);

        var operationOutcome = new OperationOutcomeViewModel()
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message
        };

        return RedirectToAction(nameof(Index), nameof(RemotePointsController), operationOutcome);
    }

}
