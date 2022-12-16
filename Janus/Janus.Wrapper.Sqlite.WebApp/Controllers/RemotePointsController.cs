using FunctionalExtensions.Base;
using Janus.Communication.Remotes;
using Janus.Wrapper.Sqlite;
using Janus.Wrapper.Sqlite.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Wrapper.Sqlite.WebApp.Controllers;
public class RemotePointsController : Controller
{
    private readonly SqliteWrapperManager _wrapperManager;
    private Logging.ILogger<RemotePointsController>? _logger;

    public RemotePointsController(SqliteWrapperManager wrapperManager, Logging.ILogger? logger = null)
    {
        _wrapperManager = wrapperManager;
        _logger = logger?.ResolveLogger<RemotePointsController>();
    }

    public IActionResult Index()
    {
        var remotePoints = _wrapperManager.GetRegisteredRemotePoints()
                                           .Map(rp => new RemotePointViewModel()
                                           {
                                               Address = rp.Address,
                                               NodeId = rp.NodeId,
                                               Port = rp.Port,
                                               RemotePointType = rp.RemotePointType
                                           });

        var operationOutcome = JsonSerializer.Deserialize<OperationOutcomeViewModel>(TempData["OperationOutcome"]?.ToString() ?? "null");
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
        var result = await _wrapperManager.RegisterRemotePoint(undeterminedRemotePoint);

        var operationOutcome = new OperationOutcomeViewModel()
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message
        };

        TempData["OperationOutcome"] = JsonSerializer.Serialize(operationOutcome);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> PingRemotePoint([FromForm] string address, [FromForm] int port)
    {
        var undeterminedRemotePoint = new UndeterminedRemotePoint(address, port);
        var result = await _wrapperManager.SendHello(undeterminedRemotePoint);

        var operationOutcome = new OperationOutcomeViewModel()
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message
        };

        TempData["OperationOutcome"] = JsonSerializer.Serialize(operationOutcome);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UnregisterRemotePoint([FromForm] string nodeId)
    {
        var remotePoint = _wrapperManager.GetRegisteredRemotePoints()
                                          .FirstOrDefault(x => x.NodeId == nodeId);
        // no such remote point found
        if(remotePoint is null)
        {
            TempData["OperationOutcome"] = JsonSerializer.Serialize(
                new OperationOutcomeViewModel()
                {
                    IsSuccess = false,
                    Message = $"No remote point with id {nodeId} registered."
                });

            return RedirectToAction(nameof(Index));
        }

        var result = await _wrapperManager.UnregisterRemotePoint(remotePoint);

        var operationOutcome = new OperationOutcomeViewModel()
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message
        };

        TempData["OperationOutcome"] = JsonSerializer.Serialize(operationOutcome);

        return RedirectToAction(nameof(Index));
    }

}
