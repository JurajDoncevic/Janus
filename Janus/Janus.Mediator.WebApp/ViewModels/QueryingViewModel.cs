namespace Janus.Mediator.WebApp.ViewModels;

public sealed class QueryingViewModel
{
    public string MediatedDataSourceJson { get; init; } = "{}";
    public string QueryText { get; init; } = string.Empty;
    public TabularDataViewModel? QueryResult { get; init; }
    public OperationOutcomeViewModel? OperationOutcome { get; init; }
}
