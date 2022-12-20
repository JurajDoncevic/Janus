namespace Janus.Mediator.WebApp.ViewModels;

public class QueryRemotePointViewModel
{
    public IEnumerable<RemotePointViewModel> RemotePoints { get; init; } = Enumerable.Empty<RemotePointViewModel>();
    public RemotePointViewModel? SelectedRemotePoint { get; init; } = null;
    public string QueryText { get; init; } = string.Empty;
    public OperationOutcomeViewModel? OperationOutcome { get; init; }
    public TabularDataViewModel? QueryResults { get; init; }
}
