using FunctionalExtensions.Base;
using Janus.Commons;
using Janus.Commons.Nodes;
using Janus.Communication.Remotes;

namespace Janus.Wrapper.Sqlite.ConsoleApp;
public class WrapperConfigurationOptions
{
    public string NodeId { get; init; }

    public int ListenPort { get; init; }

    public int TimeoutMs { get; init; }

    public string SourceConnectionString { get; init; }

    public bool AllowsCommands { get; init; }

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
    public static WrapperOptions ToWrapperOptions(this WrapperConfigurationOptions options)
        => new WrapperOptions(
            options.NodeId,
            options.ListenPort,
            options.TimeoutMs,
            options.CommunicationFormat,
            options.NetworkAdapterType,
            options.StartupRemotePoints
                   .Map(srp => new UndeterminedRemotePoint(srp.Address, srp.ListenPort))
                   .ToList(),
            options.SourceConnectionString,
            options.AllowsCommands,
            "wrapper_database.db"
            );

}
