using Janus.Communication.Remotes;

namespace Janus.Mask.WebApi.WebApp.ViewModels;

public sealed class VisibleSchemasViewModel
{
    public List<RemotePointViewModel> RegisteredRemotePoints { get; init; } = new ();
}
