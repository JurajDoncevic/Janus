using Janus.Mask.WebApi.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Janus.Mask.WebApi.WebApp.Commons.Helpers;

namespace Janus.Mask.WebApi.WebApp.Controllers;
public class QueryingController : Controller
{
    private readonly WebApiMaskManager _maskManager;
    private readonly Logging.ILogger<QueryingController>? _logger;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public QueryingController(WebApiMaskManager maskManager, JsonSerializationProvider jsonSerializationProvider, Logging.ILogger? logger)
    {
        _maskManager = maskManager;
        _jsonSerializationProvider = jsonSerializationProvider;
        _logger = logger?.ResolveLogger<QueryingController>();
    }

    public IActionResult Index()
    {
        var currentSchema = _maskManager.GetCurrentSchema()
                .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                        .Match(
                                            r => r,
                                            r => "{}"
                                        ));

        var viewModel = new QueryingViewModel()
        {
            DataSourceJson = currentSchema ? PrettyJsonString(currentSchema.Value) : "No schema generated"
        };


        return View(viewModel);
    }

    [HttpPost]
    [Route("[controller]/")]
    public async Task<IActionResult> RunQuery([FromForm] string queryText)
    {
        var stopwatch = Stopwatch.StartNew();
        var queryResult =
            await _maskManager.CreateQuery(queryText)
                .Bind(query => _maskManager.RunQuery(query));
        var timeNeeded = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        var currentSchema = _maskManager.GetCurrentSchema()
                            .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                                    .Match(
                                                        r => r,
                                                        r => "{}"
                                                    ));
        var tabularDataViewModel = new TabularDataViewModel()
        {
            ColumnDataTypes = queryResult
                                ? queryResult.Data.ColumnDataTypes.ToDictionary(_ => _.Key, _ => _.Value)
                                : new(),
            DataRows = queryResult
                        ? queryResult.Data.RowData.Map(row => row.ColumnValues.ToDictionary(cv => cv.Key, cv => cv.Value?.ToString() ?? "NULL")).ToList()
                        : new List<Dictionary<string, string>>(),
        };

        var viewModel = new QueryingViewModel()
        {
            DataSourceJson = currentSchema ? PrettyJsonString(currentSchema.Value) : "{}",
            QueryText = queryText,
            QueryResult = Option<TabularDataViewModel>.Some(tabularDataViewModel),
            OperationOutcome = Option<OperationOutcomeViewModel>.Some(
                            new OperationOutcomeViewModel()
                            {
                                IsSuccess = currentSchema && queryResult,
                                Message = currentSchema.Match(schema => "", () => "No schema generated.\n") +
                                          queryResult.Match(r => $"Query completed in {timeNeeded}ms", message => message)
                            })
        };

        return View(nameof(Index), viewModel);
    }

    public async Task<IActionResult> QueryRemotePoint()
    {
        var remotePoints =
            _maskManager.GetRegisteredRemotePoints()
            .Map(rp => new RemotePointViewModel
            {
                NodeId = rp.NodeId,
                Address = rp.Address,
                Port = rp.Port,
                RemotePointType = rp.RemotePointType
            });
        var viewModel = new QueryRemotePointViewModel
        {
            RemotePoints = remotePoints,
            SelectedRemotePoint = null,
            QueryText = string.Empty,
            OperationOutcome = Option<OperationOutcomeViewModel>.None,
            QueryResults = Option<TabularDataViewModel>.None
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> QueryRemotePoint([FromForm] string queryText, [FromForm] string nodeId)
    {
        var targetRemotePoint =
            _maskManager.GetRegisteredRemotePoints()
            .FirstOrDefault(rp => rp.NodeId.Equals(nodeId));

        var remotePoints =
            _maskManager.GetRegisteredRemotePoints()
            .Map(rp => new RemotePointViewModel
            {
                NodeId = rp.NodeId,
                Address = rp.Address,
                Port = rp.Port,
                RemotePointType = rp.RemotePointType
            });

        if (targetRemotePoint == null)
        {
            return View(new QueryRemotePointViewModel
            {
                RemotePoints = remotePoints,
                SelectedRemotePoint = remotePoints.FirstOrDefault(),
                QueryText = queryText,
                OperationOutcome = Option<OperationOutcomeViewModel>.Some(
                    new OperationOutcomeViewModel
                    {
                        IsSuccess = false,
                        Message = $"No remote point with id \"{nodeId}\""
                    })
            });
        }

        var stopwatch = Stopwatch.StartNew();
        var queryResult =
            await _maskManager.CreateQuery(queryText)
            .Bind(query => _maskManager.RunQueryOn(query, targetRemotePoint));
        var elapsedTime = stopwatch.Elapsed.Milliseconds;
        stopwatch.Stop();



        var viewModel = new QueryRemotePointViewModel
        {
            RemotePoints = remotePoints,
            SelectedRemotePoint = remotePoints.FirstOrDefault(rp => rp.NodeId.Equals(nodeId)),
            QueryText = queryText,
            OperationOutcome = Option<OperationOutcomeViewModel>.Some(
                new OperationOutcomeViewModel
                {
                    IsSuccess = queryResult.IsSuccess,
                    Message = $"{queryResult.Message}. Elapsed time: {elapsedTime}ms"
                }),
            QueryResults = queryResult.Match(
                r => Option<TabularDataViewModel>.Some(
                        new TabularDataViewModel
                        {
                            ColumnDataTypes = r.ColumnDataTypes.ToDictionary(_ => _.Key, _ => _.Value),
                            DataRows = r.RowData.Map(rd => rd.ColumnValues.ToDictionary(_ => _.Key, _ => _.Value?.ToString() ?? "NULL")).ToList()
                        }),
                msg => Option<TabularDataViewModel>.None
                )
        };

        return View(nameof(QueryRemotePoint), viewModel);
    }
}
