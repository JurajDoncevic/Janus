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
        var viewModel = JsonSerializer.Deserialize<CommandingViewModel>(TempData["CommandingViewModel"]?.ToString() ?? "null");
        if (viewModel == null)
        {
            var currentSchema = _mediatorManager.GetCurrentSchema()
                    .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                            .Match(
                                                r => r,
                                                r => "{}"
                                            ));

            viewModel = new CommandingViewModel()
            {
                MediatedDataSourceJson = currentSchema ? PrettyJsonString(currentSchema.Value) : "No schema generated"
            };
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RunCommand([FromForm] string commandText)
    {
        var commandResult =
            await _mediatorManager.CreateCommand(commandText)
                .Bind(command => _mediatorManager.RunCommand(command));

        var currentSchema = _mediatorManager.GetCurrentSchema()
                            .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                                    .Match(
                                                        r => r,
                                                        r => "{}"
                                                    ));

        var viewModel = new CommandingViewModel()
        {
            MediatedDataSourceJson = currentSchema ? currentSchema.Value : "{}",
            CommandText = commandText,
            OperationOutcome = Option<OperationOutcomeViewModel>.Some(
                new OperationOutcomeViewModel()
                {
                    IsSuccess = currentSchema && commandResult,
                    Message = currentSchema.Match(schema => "", () => "No schema generated.\n") +
                          commandResult.Match(r => "", message => message)
                })
        };

        TempData["CommandingViewModel"] = JsonSerializer.Serialize(viewModel);

        return RedirectToAction(nameof(Index));
    }
}
