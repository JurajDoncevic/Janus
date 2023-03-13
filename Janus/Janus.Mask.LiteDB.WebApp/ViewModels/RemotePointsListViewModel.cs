namespace Janus.Mask.LiteDB.WebApp.ViewModels;

public class RemotePointsListViewModel
{
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
    public List<RemotePointViewModel> RegisteredRemotePoints { get; init; } = new();
    public List<RemotePointViewModel> PersistedRemotePoints { get; init; } = new();
}
