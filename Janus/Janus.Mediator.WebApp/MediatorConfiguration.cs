using Janus.Commons;
using Janus.Commons.Nodes;
using Janus.Communication.Remotes;

namespace Janus.Mediator.WebApp;
internal class MediatorConfiguration
{
    public string NodeId { get; init; }

    public int ListenPort { get; init; }

    public int TimeoutMs { get; init; }

    public CommunicationFormats CommunicationFormat { get; init; } = CommunicationFormats.UNKNOWN;

    public NetworkAdapterTypes NetworkAdapterType { get; init; } = NetworkAdapterTypes.UNKNOWN;

    public List<RemotePointOptions> StartupRemotePoints { get; init; } = new();

    public string PersistenceConnectionString { get; init; } = "./mediator_database.db";
}

internal class RemotePointOptions
{
    public NodeTypes NodeType { get; init; }
    public int ListenPort { get; init; }
    public string Address { get; init; }
}

internal static partial class ConfigurationOptionsExtensions
{
    internal static MediatorOptions ToMediatorOptions(this MediatorConfiguration configuration)
        => new MediatorOptions(
            configuration.NodeId,
            configuration.ListenPort,
            configuration.TimeoutMs,
            configuration.CommunicationFormat,
            configuration.NetworkAdapterType,
            configuration.StartupRemotePoints
                   .Select(remotePointOptions =>
                        (RemotePoint)(remotePointOptions switch
                        {
                            { NodeType: NodeTypes.MASK } rm => new MaskRemotePoint(rm.Address, rm.ListenPort),
                            { NodeType: NodeTypes.MEDIATOR } rm => new MediatorRemotePoint(rm.Address, rm.ListenPort),
                            { NodeType: NodeTypes.WRAPPER } rm => new WrapperRemotePoint(rm.Address, rm.ListenPort)
                        }))
                   .ToList(),
            configuration.PersistenceConnectionString
            );

}
