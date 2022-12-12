using Janus.Commons.SchemaModels;
using Janus.Mediator.WebApp.ViewModels;
using Janus.Serialization.Json;
using Microsoft.AspNetCore.Mvc;

namespace Janus.Mediator.WebApp.Controllers;
public class QueryingController : Controller
{
    private readonly MediatorManager _mediatorManager;
    private readonly Logging.ILogger<QueryingController>? _logger;
    private readonly JsonSerializationProvider _jsonSerializationProvider;

    public QueryingController(MediatorManager mediatorManager, JsonSerializationProvider jsonSerializationProvider, Logging.ILogger? logger)
    {
        _mediatorManager = mediatorManager;
        _jsonSerializationProvider = jsonSerializationProvider;
        _logger = logger?.ResolveLogger<QueryingController>();
    }

    public IActionResult Index(QueryingViewModel? viewModel = null)
    {
        if (viewModel == null)
        {
            var currentSchema = _mediatorManager.GetCurrentSchema()
                    .Map(currentSchema => _jsonSerializationProvider.DataSourceSerializer.Serialize(currentSchema)
                                            .Match(
                                                r => r,
                                                r => "{}"
                                            ));

            viewModel = new QueryingViewModel()
            {
                MediatedDataSourceJson = currentSchema ? currentSchema.Value : "No schema generated"
            };
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> RunQuery([FromBody] string queryText)
    {
        var queryResult =
            await _mediatorManager.CreateQuery(queryText)
                .Bind(query => _mediatorManager.RunQuery(query));

        var currentSchema = _mediatorManager.GetCurrentSchema()
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
            MediatedDataSourceJson = currentSchema ? currentSchema.Value : "{}",
            QueryText = queryText,
            QueryResult = tabularDataViewModel,
            OperationOutcome = new OperationOutcomeViewModel()
            {
                IsSuccess = currentSchema && queryResult,
                Message = !currentSchema 
                            ? "No schema generated" 
                            : !queryResult 
                                ? queryResult.Message 
                                : "Unknown error"
            }
        };

        return RedirectToAction(nameof(Index), viewModel);
    }
}
