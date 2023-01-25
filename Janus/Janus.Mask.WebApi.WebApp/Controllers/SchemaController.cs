using Janus.Mask.WebApi.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using Results = FunctionalExtensions.Base.Resulting.Results;
using static Janus.Mask.WebApi.WebApp.Commons.Helpers;

namespace Janus.Mask.WebApi.WebApp.Controllers;
public class SchemaController : Controller
{
    private readonly WebApiMaskManager _maskManager;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public SchemaController(WebApiMaskManager maskManager, JsonSerializationProvider jsonSerializationProvider)
    {
        _maskManager = maskManager;
        _jsonSerializationProvider = jsonSerializationProvider;
    }

    public async Task<IActionResult> CurrentSchema()
    {
        var getCurrentSchema = _maskManager.GetCurrentSchema();
        var schemaToJson =
            getCurrentSchema.Match(
                                dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource).Map(PrettyJsonString),
                                () => Results.OnFailure<string>("No mediated schema generated")
                                );

        var viewModel = new DataSourceViewModel
        {
            DataSourceVersion = getCurrentSchema.Match(ds => ds.Version, () => string.Empty),
            DataSourceJson = schemaToJson.Match(data => data, message => string.Empty),
            OperationOutcome =
                schemaToJson.IsSuccess
                ? Option<OperationOutcomeViewModel>.None
                : Option<OperationOutcomeViewModel>.Some(new OperationOutcomeViewModel
                {
                    IsSuccess = false,
                    Message = schemaToJson.Message
                })
        };

        return View(viewModel);
    }

    public async Task<IActionResult> PersistedSchemas()
    {
        var persistedMediatedSchemas =
            (await _maskManager.GetAllPersistedSchemas())
                .Match(r => r, message => Enumerable.Empty<Persistence.Models.DataSourceInfo>())
                .Map(schema => new PersistedSchemaViewModel
                {
                    MediatedDataSourceVersion = schema.MediatedDataSource.Version,
                    MediatedDataSourceJson = PrettyJsonString(_jsonSerializationProvider.DataSourceSerializer.Serialize(schema.MediatedDataSource).Data ?? "{}"),
                    MediationScript = schema.MediationScript,
                    LoadedDataSourceJsons = schema.LoadedDataSources.ToDictionary(
                                                                    kv => new RemotePointViewModel
                                                                    {
                                                                        NodeId = kv.Key.NodeId,
                                                                        Address = kv.Key.Address,
                                                                        Port = kv.Key.Port,
                                                                        RemotePointType = kv.Key.RemotePointType,

                                                                    },
                                                                    kv => PrettyJsonString(_jsonSerializationProvider.DataSourceSerializer.Serialize(kv.Value).Data ?? "{}")
                                                                    ),
                    PersistedOn = schema.CreatedOn
                });

        var viewModel = new PersistedSchemaListViewModel
        {
            PersistedSchemas = persistedMediatedSchemas.ToList(),
            OperationOutcome = TempData.ToOperationOutcomeViewModel()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> PersistCurrentSchema()
    {
        if (_maskManager.GetCurrentSchema() && _maskManager.GetCurrentSchema())
        {
            var persistence =
                await _maskManager.PersistCurrentSchema(
                    _maskManager.GetCurrentSchema().Value
                    );

            TempData["Constants.IsSuccess"] = persistence.IsSuccess;
            TempData["Constants.Message"] = persistence.Message;
            return RedirectToAction(nameof(CurrentSchema));
        }
        else
        {
            TempData["Constants.IsSuccess"] = false;
            TempData["Constants.Message"] = "Can't persist a schema because no schema is generated";
            return RedirectToAction(nameof(CurrentSchema));
        }

    }

    [HttpPost]
    public async Task<IActionResult> LoadLatestSchemaFromPersistence()
    {
        var getCurrentSchema = await _maskManager.LoadLatestSchemaFromPersistence();
        var schemaToJson =
            getCurrentSchema.Bind(dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource));

        TempData["Constants.IsSuccess"] = schemaToJson.IsSuccess;
        TempData["Constants.Message"] = schemaToJson.Message;
        return RedirectToAction(nameof(PersistedSchemas));
    }

    public async Task<IActionResult> DeleteSchemaFromPersistence([FromForm] string dataSourceVersion)
    {
        var deletion =
            await _maskManager.DeleteSchema(dataSourceVersion);

        TempData["Constants.IsSuccess"] = deletion.IsSuccess;
        TempData["Constants.Message"] = deletion.Message;
        return RedirectToAction(nameof(PersistedSchemas));
    }

    public async Task<IActionResult> VisibleSchemas()
    {
        var remotePoints = _maskManager.GetRegisteredRemotePoints();

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
        var remotePoint = _maskManager.GetRegisteredRemotePoints()
                            .FirstOrDefault(rp => rp.NodeId.Equals(nodeId));

        if (remotePoint is null)
        {
            return NotFound();
        }

        var schemaResult =
            (await _maskManager.GetSchemaFrom(remotePoint))
                .Bind(_jsonSerializationProvider.DataSourceSerializer.Serialize);

        return schemaResult.Match(
            schema => (IActionResult)Content(schema, "application/json"),
            message => (IActionResult)StatusCode(500, message)
            );
    }

    [HttpGet]
    [Route("/LoadSchema/{nodeId}")]
    public async Task<IActionResult> LoadSchema(string nodeId)
    {
        var remotePoint =
            _maskManager.GetRegisteredRemotePoints()
            .FirstOrDefault(rp => rp.NodeId.Equals(nodeId));

        if (remotePoint is null)
        {
            return NotFound($"No remote point with {nodeId}");
        }

        var schemaLoading =
            (await _maskManager.LoadSchemaFrom(remotePoint))
            .Bind(_jsonSerializationProvider.DataSourceSerializer.Serialize);

        return schemaLoading.Match(
            schema => (IActionResult)Json(schema),
            message => (IActionResult)StatusCode(500, message)
            );
    }

    [HttpGet]
    [Route("/UnloadSchema/{nodeId}")]
    public async Task<IActionResult> UnloadSchema(string nodeId)
    {
        var remotePoint =
            _maskManager.GetRegisteredRemotePoints()
            .FirstOrDefault(rp => rp.NodeId.Equals(nodeId));

        if (remotePoint is null)
        {
            return NotFound($"No remote point with {nodeId}");
        }

        var schemaUnloading = _maskManager.UnloadSchemaFrom(remotePoint);

        return schemaUnloading.Match(
            message => (IActionResult)Json(message),
            message => (IActionResult)StatusCode(500, message)
            );
    }

}
