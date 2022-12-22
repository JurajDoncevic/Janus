namespace Janus.Mediator.WebApp.ViewModels;

public sealed class QueryingViewModel
{
    public string MediatedDataSourceJson { get; init; } = string.Empty;
    public string QueryText { get; init; } = string.Empty;
    public Option<TabularDataViewModel> QueryResult { get; init; }
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
}
