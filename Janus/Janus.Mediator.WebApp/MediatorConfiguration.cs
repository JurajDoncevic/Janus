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

    public bool EagerStartup { get; init; } = false;

    public List<RemotePointConfiguration> StartupRemotePoints { get; init; } = new();

    public List<string> StartupNodesSchemaLoad { get; init; } = new();

    public string StartupMediationScript { get; set; } = string.Empty;

    public string PersistenceConnectionString { get; init; } = "./mediator_database.db";
}

internal class RemotePointConfiguration
{
    public int ListenPort { get; init; }
    public string Address { get; init; } = "";
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
            configuration.EagerStartup,
            configuration.StartupRemotePoints
                   .Select(remotePointConfiguration => new UndeterminedRemotePoint(remotePointConfiguration.Address, remotePointConfiguration.ListenPort))
                   .ToList(),
            configuration.StartupNodesSchemaLoad,
            configuration.StartupMediationScript,
            configuration.PersistenceConnectionString
            );

}
