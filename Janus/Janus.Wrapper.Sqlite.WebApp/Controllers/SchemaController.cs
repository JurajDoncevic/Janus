using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Remotes;
using Janus.Wrapper.Sqlite.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using Results = FunctionalExtensions.Base.Resulting.Results;
using Janus.Wrapper.Sqlite;
using static Janus.Wrapper.Sqlite.WebApp.Commons.Helpers;
using Janus.Components.Persistence;
using Janus.Commons.SchemaModels;
using Janus.Wrapper.Persistence.Models;

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
            DataSourceVersion = getCurrentSchema.Match(ds => ds.Version, () => string.Empty),
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


    public async Task<IActionResult> PersistedSchemas()
    {
        var persistedSchemas =
            (await _wrapperManager.GetPersistedSchemas())
                .Match(r => r, msg => Enumerable.Empty<DataSourceInfo>())
                .Map(dataSourceInfo => new PersistedSchemaViewModel
                {
                    InferredDataSourceVersion = dataSourceInfo.InferredDataSource.Version,
                    InferredDataSourceJson = PrettyJsonString(_jsonSerializationProvider.DataSourceSerializer.Serialize(dataSourceInfo.InferredDataSource).Data ?? "{}"),
                    PersistedOn = dataSourceInfo.CreatedOn
                });

        var viewModel = new PersistedSchemaListViewModel
        {
            PersistedSchemas = persistedSchemas.ToList(),
            OperationOutcome = TempData.ToOperationOutcomeViewModel()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> PersistCurrentSchema()
    {
        var currentSchema = _wrapperManager.GetCurrentSchema();
        if (currentSchema)
        {
            var persistence = await _wrapperManager.PersistSchema(currentSchema.Value);

            TempData["Constants.IsSuccess"] = persistence.IsSuccess;
            TempData["Constants.Message"] = persistence.Message;
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["Constants.IsSuccess"] = false;
            TempData["Constants.Message"] = "No schema persisted because no schema is generated";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeletePersistedSchema([FromForm] string dataSourceVersion)
    {
        var deletion = await _wrapperManager.DeletePersistedSchema(dataSourceVersion);

        TempData["Constants.IsSuccess"] = deletion.IsSuccess;
        TempData["Constants.Message"] = deletion.Message;
        return RedirectToAction(nameof(PersistedSchemas));
    }
}

