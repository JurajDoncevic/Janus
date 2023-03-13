using Janus.Mask.LiteDB.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using static Janus.Mask.LiteDB.WebApp.Commons.Helpers;

namespace Janus.Mask.LiteDB.WebApp.Controllers;
public class CommandingController : Controller
{
    private readonly LiteDbMaskManager _maskManager;
    private readonly Logging.ILogger<CommandingController>? _logger;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public CommandingController(LiteDbMaskManager maskManager, JsonSerializationProvider jsonSerializationProvider, Logging.ILogger? logger)
    {
        _maskManager = maskManager;
        _jsonSerializationProvider = jsonSerializationProvider;
        _logger = logger?.ResolveLogger<CommandingController>();
    }

    public IActionResult Index()
    {

        var currentSchema = _maskManager.GetCurrentSchema()
                .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                        .Match(
                                            r => r,
                                            r => "{}"
                                        ));

        var viewModel = new CommandingViewModel()
        {
            DataSourceJson = currentSchema ? PrettyJsonString(currentSchema.Value) : "No schema generated",
            OperationOutcome = TempData.ToOperationOutcomeViewModel()
        };


        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RunCommand([FromForm] string commandText)
    {
        var commandResult =
            await _maskManager.CreateCommand(commandText)
                .Bind(command => _maskManager.RunCommand(command));


        TempData["Constants.IsSuccess"] = commandResult.IsSuccess;
        TempData["Constants.Message"] = commandResult.Message;
        return RedirectToAction(nameof(Index));
    }
}
