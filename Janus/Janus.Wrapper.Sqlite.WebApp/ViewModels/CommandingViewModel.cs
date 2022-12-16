namespace Janus.Wrapper.Sqlite.WebApp.ViewModels;

public sealed class CommandingViewModel
{
    public OperationOutcomeViewModel? OperationOutcome { get; init; }
    public string InferredDataSourceJson { get; init; } = "{}";
    public string CommandText { get; set; } = string.Empty;
}
