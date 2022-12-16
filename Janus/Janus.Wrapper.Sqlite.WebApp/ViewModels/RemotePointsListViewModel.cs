namespace Janus.Wrapper.Sqlite.WebApp.ViewModels;

public class RemotePointsListViewModel
{
    public OperationOutcomeViewModel? OperationOutcome { get; set; }
    public List<RemotePointViewModel> RemotePoints { get; set; } = new();
}
