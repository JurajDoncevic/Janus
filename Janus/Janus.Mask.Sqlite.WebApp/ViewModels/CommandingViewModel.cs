namespace Janus.Mask.Sqlite.WebApp.ViewModels;

public sealed class CommandingViewModel
{
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
    public string DataSourceJson { get; init; } = "{}";
    public string CommandText { get; set; } = string.Empty;
}
