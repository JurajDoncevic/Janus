using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Remotes;
using Janus.Wrapper.Sqlite.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using Results = FunctionalExtensions.Base.Resulting.Results;
using Janus.Wrapper.Sqlite;
using static Janus.Wrapper.Sqlite.WebApp.Commons.Helpers;

namespace Janus.Wrapper.Sqlite.WebApp.Controllers;
public class SchemaController : Controller
{
    private readonly SqliteWrapperManager _wrapperManager;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public SchemaController(SqliteWrapperManager wrapperManager, JsonSerializationProvider jsonSerializationProvider)
    {
        _wrapperManager = wrapperManager;
        _jsonSerializationProvider = jsonSerializationProvider;
    }

    public async Task<IActionResult> Index()
    {
        var getCurrentSchema = _wrapperManager.GetCurrentSchema();
        var schemaToJson =
            getCurrentSchema.Match(
                                dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource).Map(PrettyJsonString),
                                () => Results.OnFailure<string>("No inferred schema generated")
                                );

        var viewModel = new DataSourceViewModel
        {
            DataSourceJson = schemaToJson.Match(data => data, message => string.Empty),
            OperationOutcome =
                schemaToJson.IsSuccess
                ? TempData.ToOperationOutcomeViewModel()
                : Option<OperationOutcomeViewModel>.Some(new OperationOutcomeViewModel
                {
                    IsSuccess = false,
                    Message = schemaToJson.Message
                })
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateSchema()
    {
        var schemaGeneration =
            (await _wrapperManager.GenerateSchema())
                .Bind(r => _jsonSerializationProvider.DataSourceSerializer.Serialize(r));


        TempData["Constants.IsSuccess"] = schemaGeneration.IsSuccess;
        TempData["Constants.Message"] = schemaGeneration.Message;
        return RedirectToAction(nameof(Index));
    }
}
