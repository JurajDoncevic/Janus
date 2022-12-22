namespace Janus.Wrapper.Sqlite.WebApp.ViewModels;

public sealed class QueryingViewModel
{
    public string InferredDataSourceJson { get; init; } = string.Empty;
    public string QueryText { get; init; } = string.Empty;
    public Option<TabularDataViewModel> QueryResult { get; init; }
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
}
