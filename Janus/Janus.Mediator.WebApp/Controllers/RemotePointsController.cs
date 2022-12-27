using Janus.Communication.Remotes;
using Janus.Mediator.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

    public async Task<IActionResult> Index()
    {
        var registeredRemotePoints = _mediatorManager.GetRegisteredRemotePoints()
                                           .Map(rp => new RemotePointViewModel()
                                           {
                                               Address = rp.Address,
                                               NodeId = rp.NodeId,
                                               Port = rp.Port,
                                               RemotePointType = rp.RemotePointType
                                           });

        var persistedRemotePoints = (await _mediatorManager.GetPersistedRemotePoints()
                                        .Map(result => result.Map(
                                            rp => new RemotePointViewModel()
                                            {
                                                Address = rp.Address,
                                                NodeId = rp.NodeId,
                                                Port = rp.Port,
                                                RemotePointType = rp.RemotePointType
                                            })))
                                        .Match(r => r, message => Enumerable.Empty<RemotePointViewModel>());

        var viewModel = new RemotePointsListViewModel()
        {
            RegisteredRemotePoints = registeredRemotePoints.ToList(),
            PersistedRemotePoints = persistedRemotePoints.ToList(),
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
        if (remotePoint is null)
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

    [HttpPost]
    public async Task<IActionResult> PersistRemotePoint([FromForm]RemotePointViewModel viewModel)
    {

        RemotePoint remotePoint = viewModel switch
        {
            { RemotePointType: RemotePointTypes.MASK } => new MaskRemotePoint(viewModel.NodeId, viewModel.Address, viewModel.Port),
            { RemotePointType: RemotePointTypes.MEDIATOR } => new MediatorRemotePoint(viewModel.NodeId, viewModel.Address, viewModel.Port),
            { RemotePointType: RemotePointTypes.WRAPPER } => new WrapperRemotePoint(viewModel.NodeId, viewModel.Address, viewModel.Port),
            { RemotePointType: RemotePointTypes.UNDETERMINED } => new UndeterminedRemotePoint(viewModel.Address, viewModel.Port) // shouldn't reach this
        };
        var result = await _mediatorManager.PersistRemotePoint(remotePoint);

        TempData["Constants.IsSuccess"] = result.IsSuccess;
        TempData["Constants.Message"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRemotePoint([FromForm] string nodeId)
    {
        var result = await _mediatorManager.DeleteRemotePoint(nodeId);

        TempData["Constants.IsSuccess"] = result.IsSuccess;
        TempData["Constants.Message"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

}
