using Janus.Commons.SchemaModels;
using Janus.Wrapper.Sqlite.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using System.Text.Json;
using Janus.Wrapper;
using Janus.Wrapper.Sqlite;
using FunctionalExtensions.Base.Resulting;
using FunctionalExtensions.Base;

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
        var viewModel = JsonSerializer.Deserialize<QueryingViewModel>(TempData["QueryingViewModel"]?.ToString() ?? "null");

        if (viewModel == null)
        {
            var currentSchema = _wrapperManager.GetCurrentSchema()
                    .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                            .Match(
                                                r => r,
                                                r => "{}"
                                            ));

            viewModel = new QueryingViewModel()
            {
                InferredDataSourceJson = currentSchema ? currentSchema.Value : "{}"
            };
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RunQuery([FromForm] string queryText)
    {
        var queryResult =
            await _wrapperManager.CreateQuery(queryText)
                .Bind(query => _wrapperManager.RunQuery(query));

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
            InferredDataSourceJson = currentSchema ? currentSchema.Value : "{}",
            QueryText = queryText,
            QueryResult = tabularDataViewModel,
            OperationOutcome = new OperationOutcomeViewModel()
            {
                IsSuccess = currentSchema && queryResult,
                Message = currentSchema.Match(schema => "", () => "No schema generated.\n") +
                          queryResult.Match(r => "", message => message)
            }
        };

        TempData["QueryingViewModel"] = JsonSerializer.Serialize(viewModel);

        return RedirectToAction(nameof(Index));
    }
}
