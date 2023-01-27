namespace Janus.Mask.WebApi.WebApp.ViewModels;

public class SchemaLoadingViewModel
{
    public Option<DataSourceViewModel> CurrentLoadedSchema { get; init; } = Option<DataSourceViewModel>.None;
    public VisibleSchemasViewModel VisibleSchemas { get; init; }
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; } = Option<OperationOutcomeViewModel>.None;
}
