using Janus.Commons;
using Janus.Commons.Nodes;
using Janus.Communication.Remotes;

namespace Janus.Mask.WebApi.WebApp;
internal class MaskConfiguration
{
    public string NodeId { get; init; }

    public int ListenPort { get; init; }

    public int TimeoutMs { get; init; }

    public CommunicationFormats CommunicationFormat { get; init; } = CommunicationFormats.UNKNOWN;

    public NetworkAdapterTypes NetworkAdapterType { get; init; } = NetworkAdapterTypes.UNKNOWN;

    public List<RemotePointConfiguration> StartupRemotePoints { get; init; } = new();

    public string PersistenceConnectionString { get; init; } = "./mediator_database.db";
    public int WebApiPort { get; init; }
}

internal class RemotePointConfiguration
{
    public int ListenPort { get; init; }
    public string Address { get; init; } = "";
}

internal static partial class ConfigurationOptionsExtensions
{
    internal static WebApiMaskOptions ToMaskOptions(this MaskConfiguration configuration)
        => new WebApiMaskOptions(
            configuration.NodeId,
            configuration.ListenPort,
            configuration.TimeoutMs,
            configuration.CommunicationFormat,
            configuration.NetworkAdapterType,
            configuration.StartupRemotePoints
                   .Select(remotePointConfiguration => new UndeterminedRemotePoint(remotePointConfiguration.Address, remotePointConfiguration.ListenPort))
                   .ToList(),
            configuration.PersistenceConnectionString,
            configuration.WebApiPort
            );

}
