using Janus.Base.Resulting;
using Janus.Mediator.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static Janus.Mediator.WebApp.Commons.Helpers;

namespace Janus.Mediator.WebApp.Controllers;
public class CommandingController : Controller
{
    private readonly MediatorManager _mediatorManager;
    private readonly Logging.ILogger<CommandingController>? _logger;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public CommandingController(MediatorManager mediatorManager, JsonSerializationProvider jsonSerializationProvider, Logging.ILogger? logger)
    {
        _mediatorManager = mediatorManager;
        _jsonSerializationProvider = jsonSerializationProvider;
        _logger = logger?.ResolveLogger<CommandingController>();
    }

    public IActionResult Index()
    {

        var currentSchema = _mediatorManager.GetCurrentSchema()
                .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                        .Match(
                                            r => r,
                                            r => "{}"
                                        ));

        var viewModel = new CommandingViewModel()
        {
            MediatedDataSourceJson = currentSchema ? PrettyJsonString(currentSchema.Value) : "No schema generated",
            OperationOutcome = TempData.ToOperationOutcomeViewModel()
        };


        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RunCommand([FromForm] string commandText)
    {
        var commandResult =
            await _mediatorManager.CreateCommand(commandText)
                .Bind(command => _mediatorManager.RunCommand(command));


        TempData["Constants.IsSuccess"] = commandResult.IsSuccess;
        TempData["Constants.Message"] = commandResult.Message;
        return RedirectToAction(nameof(Index));
    }
}
