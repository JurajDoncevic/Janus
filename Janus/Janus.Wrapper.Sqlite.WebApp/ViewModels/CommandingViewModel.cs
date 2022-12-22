namespace Janus.Wrapper.Sqlite.WebApp.ViewModels;

public sealed class CommandingViewModel
{
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
    public string InferredDataSourceJson { get; init; } = string.Empty;
    public string CommandText { get; set; } = string.Empty;
}
