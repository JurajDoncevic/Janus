namespace Janus.Mask.WebApi.WebApp.ViewModels;

public sealed class QueryingViewModel
{
    public string DataSourceJson { get; init; } = string.Empty;
    public string QueryText { get; init; } = string.Empty;
    public Option<TabularDataViewModel> QueryResult { get; init; }
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
}
