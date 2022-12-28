namespace Janus.Mediator.WebApp.ViewModels;

public class PersistedSchemaListViewModel
{
    public List<PersistedSchemaViewModel> PersistedSchemas { get; init; } = new();
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
}
