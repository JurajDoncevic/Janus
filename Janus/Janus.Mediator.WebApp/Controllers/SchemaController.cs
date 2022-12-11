using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Remotes;
using Janus.Mediator.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;

namespace Janus.Mediator.WebApp.Controllers;
public class SchemaController : Controller
{
    private readonly MediatorManager _mediatorManager;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public SchemaController(MediatorManager mediatorManager, JsonSerializationProvider jsonSerializationProvider)
    {
        _mediatorManager = mediatorManager;
    }

    public async Task<IActionResult> Index()
    {
        var getCurrentSchema = await _mediatorManager.GetSchema();
        var schemaToJson =
            getCurrentSchema.Bind(dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource));

        return View(schemaToJson.Match(
            json => new DataSourceViewModel() { DataSourceJson = json },
            message => new DataSourceViewModel() { Message = message }
            ));
    }

    public async Task<IActionResult> LoadFromPersistence()
    {
        var getCurrentSchema = await _mediatorManager.LoadMediatedSchemaFromPersistence();
        var schemaToJson =
            getCurrentSchema.Bind(dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource));

        return schemaToJson.Match(
            json => View(json),
            message => View(message)
            );
    }

    public async Task<IActionResult> VisibleSchemas()
    {
        var visibleSchemaJsonsOrFailMessages =
            (await _mediatorManager.GetAvailableSchemas())
            .ToDictionary(kv => kv.Key, kv => kv.Value.Bind(_ => _jsonSerializationProvider.DataSourceSerializer.Serialize(_)))
            .ToDictionary(kv => kv.Key, kv => kv.Value ? kv.Value.Data : kv.Value.Message);

        return View(new VisibleSchemasViewModel()
        {
            VisibleDataSourcesJsons = visibleSchemaJsonsOrFailMessages
        });
    }

    public IActionResult LoadedSchemas()
    {
        // gets all schemas loaded schemas available for mediation
        return View();
    }

    public IActionResult LoadSchemas(IEnumerable<RemotePoint> remotePoints)
    {
        // load all schemas from selected remote points
        return View();
    }

    public IActionResult CurrentMediation()
    {
        // gets the current mediation script, make a GET and POST
        return View();
    }

    public IActionResult CurrentMediation(string mediationScript)
    {
        // sets the current mediation of loaded schemas with a mediation script
        return View();
    }
}
