using Janus.Wrapper.Sqlite.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Janus.Wrapper.Sqlite.WebApp.Commons.Helpers;

namespace Janus.Wrapper.Sqlite.WebApp.Controllers;
public class QueryingController : Controller
{
    private readonly SqliteWrapperManager _wrapperManager;
    private readonly Logging.ILogger<QueryingController>? _logger;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public QueryingController(SqliteWrapperManager wrapperManager, JsonSerializationProvider jsonSerializationProvider, Logging.ILogger? logger)
    {
        _wrapperManager = wrapperManager;
        _jsonSerializationProvider = jsonSerializationProvider;
        _logger = logger?.ResolveLogger<QueryingController>();
    }

    public IActionResult Index()
    {
        var currentSchema = _wrapperManager.GetCurrentSchema()
                .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                        .Match(
                                            r => r,
                                            r => "{}"
                                        ));

        var viewModel = new QueryingViewModel()
        {
            InferredDataSourceJson = currentSchema ? PrettyJsonString(currentSchema.Value) : "No schema generated"
        };


        return View(viewModel);
    }

    [HttpPost]
    [Route("[controller]/")]
    public async Task<IActionResult> RunQuery([FromForm] string queryText)
    {
        var stopwatch = Stopwatch.StartNew();
        var queryResult =
            await _wrapperManager.CreateQuery(queryText)
                .Bind(query => _wrapperManager.RunQuery(query));
        var timeNeeded = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        var currentSchema = _wrapperManager.GetCurrentSchema()
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
            InferredDataSourceJson = currentSchema ? PrettyJsonString(currentSchema.Value) : "{}",
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
}
