using Janus.Communication.Remotes;

namespace Janus.Components.Core;

public interface IComponentOptions
{
    public string NodeId { get; }
    public int ListenPort { get; }
    public int TimeoutMs { get; }
    public List<RemotePoint> StartupRemotePoints { get; }
}