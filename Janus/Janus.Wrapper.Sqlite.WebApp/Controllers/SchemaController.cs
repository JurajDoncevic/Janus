using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Remotes;
using Janus.Wrapper.Sqlite.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using Results = FunctionalExtensions.Base.Resulting.Results;
using Janus.Wrapper.Sqlite;

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
                                dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource),
                                () => Results.OnFailure<string>("No schema loaded")
                                );

        return View(schemaToJson.Match(
            json => new DataSourceViewModel() { DataSourceJson = json },
            message => new DataSourceViewModel() { Message = message }
            ));
    }

    public async Task<IActionResult> GenerateSchema()
    {
        var schemaGeneration = 
            (await _wrapperManager.GenerateSchema())
                .Bind(r => _jsonSerializationProvider.DataSourceSerializer.Serialize(r));     
                        

        return schemaGeneration.Match(
            r => (IActionResult)Json(r),
            message => (IActionResult)StatusCode(500, message)
            );
    }

    public async Task<IActionResult> LoadFromPersistence()
    {
        //var getCurrentSchema = await _wrapperManager.();
        //var schemaToJson =
        //    getCurrentSchema.Bind(dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource));

        //return schemaToJson.Match(
        //    json => View(json),
        //    message => View(message)
        //    );

        return NotFound();
    }
}
