﻿using Janus.Commons;
using Janus.Commons.Nodes;
using Janus.Communication.Remotes;

namespace Janus.Wrapper.Sqlite.WebApp;
internal class WrapperConfiguration
{
    public string NodeId { get; init; }

    public int ListenPort { get; init; }

    public int TimeoutMs { get; init; }

    public CommunicationFormats CommunicationFormat { get; init; } = CommunicationFormats.UNKNOWN;

    public NetworkAdapterTypes NetworkAdapterType { get; init; } = NetworkAdapterTypes.UNKNOWN;

    public List<RemotePointConfiguration> StartupRemotePoints { get; init; } = new();

    public string PersistenceConnectionString { get; init; } = "./mediator_database.db";

    public bool AllowCommands { get; init; }
    public string? DataSourceName { get; set; }

    public string SourceConnectionString { get; init; } = string.Empty;
}

internal class RemotePointConfiguration
{
    public int ListenPort { get; init; }
    public string Address { get; init; } = string.Empty;
}

internal static partial class ConfigurationOptionsExtensions
{
    internal static WrapperOptions ToWrapperOptions(this WrapperConfiguration configuration)
        => new WrapperOptions(
            configuration.NodeId,
            configuration.ListenPort,
            configuration.TimeoutMs,
            configuration.CommunicationFormat,
            configuration.NetworkAdapterType,
            configuration.StartupRemotePoints
                   .Select(remotePointConfiguration => new UndeterminedRemotePoint(remotePointConfiguration.Address, remotePointConfiguration.ListenPort))
                   .ToList(),
            configuration.SourceConnectionString,
            configuration.AllowCommands,
            configuration.PersistenceConnectionString
            );

}
