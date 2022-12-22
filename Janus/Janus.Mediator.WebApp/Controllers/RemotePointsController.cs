using Janus.Communication.Remotes;
using Janus.Mediator.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Janus.Mediator.WebApp.Commons.Helpers;

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

        var viewModel = new RemotePointsListViewModel()
        {
            RemotePoints = remotePoints.ToList(),
            OperationOutcome = TempData.ToOperationOutcomeViewModel()
        };

        return View(viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> RegisterRemotePoint([FromForm] string address, [FromForm] int port)
    {
        var undeterminedRemotePoint = new UndeterminedRemotePoint(address, port);
        var result = await _mediatorManager.RegisterRemotePoint(undeterminedRemotePoint);

        TempData["Constants.IsSuccess"] = result.IsSuccess;
        TempData["Constants.Message"] = result.Message;
        return RedirectToAction(nameof(Index));
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

        TempData["Constants.IsSuccess"] = result.IsSuccess;
        TempData["Constants.Message"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UnregisterRemotePoint([FromForm] string nodeId)
    {
        var remotePoint = _mediatorManager.GetRegisteredRemotePoints()
                                          .FirstOrDefault(x => x.NodeId == nodeId);
        // no such remote point found
        if(remotePoint is null)
        {
            TempData["Constants.IsSuccess"] = false;
            TempData["Constants.Message"] = $"No remote point with id {nodeId} registered.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _mediatorManager.UnregisterRemotePoint(remotePoint);

        TempData["Constants.IsSuccess"] = result.IsSuccess;
        TempData["Constants.Message"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

}
