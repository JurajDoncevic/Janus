namespace Janus.Mediator.WebApp.ViewModels;

public class QueryRemotePointViewModel
{
    public IEnumerable<RemotePointViewModel> RemotePoints { get; init; } = Enumerable.Empty<RemotePointViewModel>();
    public RemotePointViewModel? SelectedRemotePoint { get; set; } = null;
    public string QueryText { get; init; } = string.Empty;
    public Option<OperationOutcomeViewModel> OperationOutcome { get; init; }
    public Option<TabularDataViewModel> QueryResults { get; init; }
}
