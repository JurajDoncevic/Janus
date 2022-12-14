using FunctionalExtensions.Base;
using Janus.Commons;
using Janus.Commons.Nodes;
using Janus.Communication.Remotes;

namespace Janus.Mediator.ConsoleApp;
public class MediatorConfigurationOptions
{
    public string NodeId { get; init; }

    public int ListenPort { get; init; }

    public int TimeoutMs { get; init; }

    public CommunicationFormats CommunicationFormat { get; init; } = CommunicationFormats.UNKNOWN;

    public NetworkAdapterTypes NetworkAdapterType { get; init; } = NetworkAdapterTypes.UNKNOWN;

    public List<RemotePointOptions> StartupRemotePoints { get; init; } = new();
}

public class RemotePointOptions
{
    public NodeTypes NodeType { get; init; }
    public int ListenPort { get; init; }
    public string Address { get; init; }
}

public static partial class ConfigurationOptionsExtensions
{
    public static MediatorOptions ToMediatorOptions(this MediatorConfigurationOptions options)
        => new MediatorOptions(
            options.NodeId,
            options.ListenPort,
            options.TimeoutMs,
            options.CommunicationFormat,
            options.NetworkAdapterType,
            options.StartupRemotePoints
                   .Map(srp => new UndeterminedRemotePoint(srp.Address, srp.ListenPort))
                   .ToList(),
            "./mediator_database.db"
            );

}
