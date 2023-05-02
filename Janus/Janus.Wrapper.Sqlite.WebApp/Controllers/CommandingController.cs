using Janus.Wrapper.Sqlite.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Janus.Wrapper.Sqlite;
using Janus.Base;
using Janus.Base.Resulting;
using static Janus.Wrapper.Sqlite.WebApp.Commons.Helpers;

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

        var currentSchema = _wrapperManager.GetCurrentSchema()
                .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                        .Match(
                                            r => r,
                                            r => "{}"
                                        ));

        var viewModel = new CommandingViewModel()
        {
            InferredDataSourceJson = currentSchema ? PrettyJsonString(currentSchema.Value) : "No schema generated",
            OperationOutcome = TempData.ToOperationOutcomeViewModel()
        };


        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RunCommand([FromForm] string commandText)
    {
        var commandResult =
            await _wrapperManager.CreateCommand(commandText)
                .Bind(command => _wrapperManager.RunCommand(command));


        TempData["Constants.IsSuccess"] = commandResult.IsSuccess;
        TempData["Constants.Message"] = commandResult.Message;
        return RedirectToAction(nameof(Index));
    }
}
