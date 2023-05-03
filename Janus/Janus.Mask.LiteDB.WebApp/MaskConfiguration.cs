using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Mask.LiteDB;

namespace Janus.Mask.LiteDB.WebApp;
internal class MaskConfiguration
{
    public string NodeId { get; init; }

    public int ListenPort { get; init; }

    public int TimeoutMs { get; init; }

    public CommunicationFormats CommunicationFormat { get; init; } = CommunicationFormats.UNKNOWN;

    public NetworkAdapterTypes NetworkAdapterType { get; init; } = NetworkAdapterTypes.UNKNOWN;

    public bool EagerStartup { get; init; } = false;

    public List<RemotePointConfiguration> StartupRemotePoints { get; init; } = new();

    public string StartupNodeSchemaLoad { get; init; } = string.Empty;

    public string PersistenceConnectionString { get; init; } = "./mask_database.db";

    public MaterializationConfiguration MaterializationConfiguration { get; init; } = new MaterializationConfiguration
    {
        ConnectionString = "materialized.db"
    };
}

internal class MaterializationConfiguration
{
    public string ConnectionString { get; init; } = string.Empty;
}

internal class RemotePointConfiguration
{
    public int ListenPort { get; init; }
    public string Address { get; init; } = "";
}

internal static partial class ConfigurationOptionsExtensions
{
    internal static LiteDbMaskOptions ToMaskOptions(this MaskConfiguration configuration)
        => new LiteDbMaskOptions(
            configuration.NodeId,
            configuration.ListenPort,
            configuration.TimeoutMs,
            configuration.CommunicationFormat,
            configuration.NetworkAdapterType,
            configuration.EagerStartup,
            configuration.StartupRemotePoints
                   .Select(remotePointConfiguration => new UndeterminedRemotePoint(remotePointConfiguration.Address, remotePointConfiguration.ListenPort))
                   .ToList(),
            configuration.StartupNodeSchemaLoad,
            configuration.PersistenceConnectionString,
            configuration.MaterializationConfiguration.ConnectionString
            );

}
