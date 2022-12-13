using Janus.Communication.Remotes;

namespace Janus.Mediator.WebApp.ViewModels;

public sealed class VisibleSchemasViewModel
{
    public List<RemotePointViewModel> RegisteredRemotePoints { get; init; } = new ();
}
