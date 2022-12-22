﻿using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Remotes;
using Janus.MediationLanguage;
using Janus.Mediator.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using Results = FunctionalExtensions.Base.Resulting.Results;
using static Janus.Mediator.WebApp.Commons.Helpers;

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

    public async Task<IActionResult> CurrentSchema()
    {
        var getCurrentSchema = _mediatorManager.GetCurrentSchema();
        var schemaToJson =
            getCurrentSchema.Match(
                                dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource).Map(PrettyJsonString),
                                () => Results.OnFailure<string>("No mediated schema generated")
                                );

        var viewModel = new DataSourceViewModel
        {
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

    public async Task<IActionResult> LoadFromPersistence()
    {
        var getCurrentSchema = await _mediatorManager.LoadMediatedSchemaFromPersistence();
        var schemaToJson =
            getCurrentSchema.Bind(dataSource => _jsonSerializationProvider.DataSourceSerializer.Serialize(dataSource));

        var viewModel = new DataSourceViewModel
        {
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

        return View(nameof(CurrentSchema), viewModel);
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

        if (remotePoint is null)
        {
            return NotFound();
        }

        var schemaResult =
            (await _mediatorManager.GetSchemaFrom(remotePoint))
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
            _mediatorManager.GetRegisteredRemotePoints()
            .FirstOrDefault(rp => rp.NodeId.Equals(nodeId));

        if (remotePoint is null)
        {
            return NotFound($"No remote point with {nodeId}");
        }
        
        var schemaLoading = 
            (await _mediatorManager.LoadSchemaFrom(remotePoint))
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
            _mediatorManager.GetRegisteredRemotePoints()
            .FirstOrDefault(rp => rp.NodeId.Equals(nodeId));

        if (remotePoint is null)
        {
            return NotFound($"No remote point with {nodeId}");
        }

        var schemaUnloading = _mediatorManager.UnloadSchemaFrom(remotePoint);

        return schemaUnloading.Match(
            message => (IActionResult)Json(message),
            message => (IActionResult)StatusCode(500, message)
            );
    }

    [Route("[controller]/SchemaMediation")]
    public IActionResult SchemaMediation()
    {
        var registeredRemotePoints = _mediatorManager.GetRegisteredRemotePoints();
        var loadedSchemasFromRemotePoints = _mediatorManager.GetLoadedSchemas();
        var remotePointsWithLoadedSchemas = loadedSchemasFromRemotePoints.Keys;


        // gets the current mediation script, make a GET and POST
        var viewModel = new SchemaMediationViewModel
        {
            AvailableRemotePoints =
                registeredRemotePoints
                .Except(remotePointsWithLoadedSchemas)
                .Map(rp => new RemotePointViewModel { NodeId = rp.NodeId, Address = rp.Address, Port = rp.Port, RemotePointType = rp.RemotePointType })
                .ToList(),
            LoadedDataSourceSchemas =
                loadedSchemasFromRemotePoints
                .Map(kv => _jsonSerializationProvider.DataSourceSerializer.Serialize(kv.Value).Map(PrettyJsonString)
                            .Match(
                                r => (remotePoint: kv.Key, dataSourceJson: r, message: string.Empty),
                                message => (remotePoint: kv.Key, dataSourceJson: string.Empty, message: message)
                            ))
                .ToDictionary(
                    t => new RemotePointViewModel { NodeId = t.remotePoint.NodeId, Address = t.remotePoint.Address, Port = t.remotePoint.Port, RemotePointType = t.remotePoint.RemotePointType },
                    t => new DataSourceViewModel
                    {
                        DataSourceJson = t.dataSourceJson,
                        OperationOutcome = 
                            string.IsNullOrWhiteSpace(t.message)
                            ? Option<OperationOutcomeViewModel>.None
                            : Option<OperationOutcomeViewModel>.Some(new OperationOutcomeViewModel
                            {
                                IsSuccess= string.IsNullOrWhiteSpace(t.message),
                                Message = t.message,
                            })
                    }),

            SchemaMediationScript = _mediatorManager.GetCurrentSchemaMediation().Match(
                                        mediation => mediation.ToMediationScript(),
                                        () => string.Empty
                                        ),
            OperationOutcome = TempData.ToOperationOutcomeViewModel()
        };
        return View(viewModel);
    }

    [HttpPost]
    [Route("[controller]/SchemaMediation")]
    public async Task<IActionResult> ApplyMediationScript([FromForm]string schemaMediationScript)
    {
        var registeredRemotePoints = _mediatorManager.GetRegisteredRemotePoints();
        var loadedSchemasFromRemotePoints = _mediatorManager.GetLoadedSchemas();
        var remotePointsWithLoadedSchemas = loadedSchemasFromRemotePoints.Keys;

        var mediationResult = 
            await Task.FromResult(_mediatorManager.CreateDataSourceMediation(schemaMediationScript))
                      .Bind(mediation => _mediatorManager.ApplyMediation(mediation))
                      .Bind(mediatedDataSource => Task.FromResult(_jsonSerializationProvider.DataSourceSerializer.Serialize(mediatedDataSource)));

        TempData["Constants.IsSuccess"] = mediationResult.IsSuccess;
        TempData["Constants.Message"] = mediationResult.Message;
        return RedirectToAction(nameof(SchemaMediation));
    }
}
