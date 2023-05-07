using Janus.Mask.Sqlite.WebApp.Commons;
using Janus.Mask.Sqlite.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using static Janus.Mask.Sqlite.WebApp.Commons.Helpers;
using Results = Janus.Base.Resulting.Results;

namespace Janus.Mask.Sqlite.WebApp.Controllers;
public class SchemaController : Controller
{
    private readonly SqliteMaskManager _maskManager;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public SchemaController(SqliteMaskManager maskManager, JsonSerializationProvider jsonSerializationProvider)
    {
        _maskManager = maskManager;
        _jsonSerializationProvider = jsonSerializationProvider;
    }

    public async Task<IActionResult> CurrentSchema()
    {
        var currentSchema = _maskManager.GetCurrentSchema();
        var schemaToJson =
            currentSchema.Match(
                                dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource).Map(PrettyJsonString),
                                () => Results.OnFailure<string>("No schema generated")
                                );

        var viewModel = new DataSourceViewModel
        {
            DataSourceVersion = currentSchema.Match(ds => ds.Version, () => string.Empty),
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

    public async Task<IActionResult> SchemaLoading()
    {
        var currentSchema = _maskManager.GetCurrentSchema();
        var schemaToJson =
            currentSchema.Match(
                        dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource).Map(PrettyJsonString),
                        () => Results.OnFailure<string>("No schema loaded")
                        );
        var viewModel = new SchemaLoadingViewModel
        {
            CurrentLoadedSchema = currentSchema.Map(_ => new DataSourceViewModel
            {
                DataSourceVersion = currentSchema.Match(ds => ds.Version, () => string.Empty),
                DataSourceJson = schemaToJson.Match(data => data, message => string.Empty)
            }),
            VisibleSchemas = new VisibleSchemasViewModel
            {
                RegisteredRemotePoints = _maskManager.GetRegisteredRemotePoints()
                                            .Map(rp => new RemotePointViewModel
                                            {
                                                NodeId = rp.NodeId,
                                                Address = rp.Address,
                                                Port = rp.Port,
                                                RemotePointType = rp.RemotePointType
                                            }).ToList()
            },
            OperationOutcome = Helpers.ToOperationOutcomeViewModel(TempData)
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
                    DataSourceVersion = schema.InferredDataSource.Version,
                    DataSourceJson = PrettyJsonString(_jsonSerializationProvider.DataSourceSerializer.Serialize(schema.InferredDataSource).Data ?? "{}"),
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
    [Route("/UnloadSchema/")]
    public async Task<IActionResult> UnloadSchema()
    {

        var schemaUnloading = _maskManager.UnloadSchema();

        return schemaUnloading.Match(
            message => (IActionResult)Json(message),
            message => (IActionResult)StatusCode(500, message)
            );
    }

}
