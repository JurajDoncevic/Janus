using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Remotes;
using Janus.Mediator.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using Results = FunctionalExtensions.Base.Resulting.Results;

namespace Janus.Mediator.WebApp.Controllers;
public class SchemaController : Controller
{
    private readonly MediatorManager _mediatorManager;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public SchemaController(MediatorManager mediatorManager, JsonSerializationProvider jsonSerializationProvider)
    {
        _mediatorManager = mediatorManager;
        _jsonSerializationProvider = jsonSerializationProvider;
    }

    public async Task<IActionResult> Index()
    {
        var getCurrentSchema = _mediatorManager.GetCurrentSchema();
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
        var remotePoints = _mediatorManager.GetRegisteredRemotePoints();

        return View(new VisibleSchemasViewModel()
        {
            RegisteredRemotePoints = remotePoints.Map(rp => new RemotePointViewModel()
            {
                NodeId = rp.NodeId,
                Address = rp.Address,
                Port = rp.Port,
                RemotePointType = rp.RemotePointType
            }).ToList()
        });
    }

    [HttpGet]
    [Route("/GetSchema/{nodeId}")]
    public async Task<IActionResult> GetSchema(string nodeId)
    {
        var remotePoint = _mediatorManager.GetRegisteredRemotePoints()
                            .FirstOrDefault(rp => rp.NodeId.Equals(nodeId));

        if(remotePoint is null)
        {
            return NotFound();
        }

        var schemaResult = 
            (await _mediatorManager.GetSchemaFrom(remotePoint))
                .Bind(dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource));

        return schemaResult.Match(
            schema => (IActionResult)Json(schema),
            message => (IActionResult)StatusCode(500, message)
            );
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

    public IActionResult Mediation()
    {
        // gets the current mediation script, make a GET and POST
        return View();
    }

    public IActionResult Mediation(string mediationScript)
    {
        // sets the current mediation of loaded schemas with a mediation script
        return View();
    }
}
