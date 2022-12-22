namespace Janus.Mediator.WebApp.ViewModels;

public class RemotePointsListViewModel
{
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
    public List<RemotePointViewModel> RemotePoints { get; init; } = new();
}
