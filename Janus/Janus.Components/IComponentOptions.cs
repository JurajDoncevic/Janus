using Janus.Commons;
using Janus.Communication.Remotes;

namespace Janus.Components;

public interface IComponentOptions
{
    /// <summary>
    /// Component's node id
    /// </summary>
    public string NodeId { get; }
    /// <summary>
    /// Component's listen port
    /// </summary>
    public int ListenPort { get; }
    /// <summary>
    /// Components general timeout in milliseconds
    /// </summary>
    public int TimeoutMs { get; }
    /// <summary>
    /// Component's data format for communication. Must be compatible with NetworkAdaptertype <see cref="Utils.IsDataFormatCompatibleWithAdapter(CommunicationFormats, NetworkAdapterTypes)"/>
    /// </summary>
    public CommunicationFormats CommunicationFormat { get; }
    /// <summary>
    /// Type of network adapter
    /// </summary>
    public NetworkAdapterTypes NetworkAdapterType { get; }
    /// <summary>
    /// Remote points to register on startup
    /// </summary>
    public IReadOnlyList<RemotePoint> StartupRemotePoints { get; }
}