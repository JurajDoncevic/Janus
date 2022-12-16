using Janus.Wrapper.Sqlite.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Janus.Wrapper.Sqlite;
using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;

namespace Janus.Wrapper.Sqlite.WebApp.Controllers;
public class CommandingController : Controller
{
    private readonly SqliteWrapperManager _wrapperManager;
    private readonly Logging.ILogger<CommandingController>? _logger;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public CommandingController(SqliteWrapperManager wrapperManager, JsonSerializationProvider jsonSerializationProvider, Logging.ILogger? logger)
    {
        _wrapperManager = wrapperManager;
        _jsonSerializationProvider = jsonSerializationProvider;
        _logger = logger?.ResolveLogger<CommandingController>();
    }

    public IActionResult Index()
    {
        var viewModel = JsonSerializer.Deserialize<CommandingViewModel>(TempData["CommandingViewModel"]?.ToString() ?? "null");
        if (viewModel == null)
        {
            var currentSchema = _wrapperManager.GetCurrentSchema()
                    .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                            .Match(
                                                r => r,
                                                r => "{}"
                                            ));

            viewModel = new CommandingViewModel()
            {
                InferredDataSourceJson = currentSchema ? currentSchema.Value : "{}"
            };
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RunCommand([FromForm] string commandText)
    {
        var commandResult =
            await _wrapperManager.CreateCommand(commandText)
                .Bind(command => _wrapperManager.RunCommand(command));

        var currentSchema = _wrapperManager.GetCurrentSchema()
                            .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                                    .Match(
                                                        r => r,
                                                        r => "{}"
                                                    ));

        var viewModel = new CommandingViewModel()
        {
            InferredDataSourceJson = currentSchema ? currentSchema.Value : "{}",
            CommandText = commandText,
            OperationOutcome = new OperationOutcomeViewModel()
            {
                IsSuccess = currentSchema && commandResult,
                Message = currentSchema.Match(schema => "", () => "No schema generated.\n") +
                          commandResult.Match(r => "", message => message)
            }
        };

        TempData["CommandingViewModel"] = JsonSerializer.Serialize(viewModel);

        return RedirectToAction(nameof(Index));
    }
}
