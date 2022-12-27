namespace Janus.Wrapper.Sqlite.WebApp.ViewModels;

public class RemotePointsListViewModel
{
    public Option<OperationOutcomeViewModel> OperationOutcome { get; set; }
    public List<RemotePointViewModel> RegisteredRemotePoints { get; set; } = new();
    public List<RemotePointViewModel> PersistedRemotePoints { get; set; } = new();
}
