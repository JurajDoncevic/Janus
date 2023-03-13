namespace Janus.Mask.LiteDB.WebApp.ViewModels;

public sealed class DataSourceViewModel
{
    public string DataSourceVersion { get; init; } = string.Empty;
    public string DataSourceJson { get; init; } = "{}";

    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; } = Option<OperationOutcomeViewModel>.None;
}
