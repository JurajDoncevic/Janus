﻿using Janus.Commons;
using Janus.Communication.Remotes;

namespace Janus.Mask.WebApi.WebApp;
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

    public bool StartupWebApi { get; init; } = false;

    public string PersistenceConnectionString { get; init; } = "./mask_database.db";

    public WebApiConfiguration WebApiConfiguration { get; init; } = new WebApiConfiguration
    {
        ListenPort = 80001,
        UseSSL = false,
        ListenPortSecure = null
    };
}

internal class WebApiConfiguration
{
    public int ListenPort { get; set; }
    public int? ListenPortSecure { get; set; }
    public bool UseSSL { get; set; }
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
            configuration.EagerStartup,
            configuration.StartupRemotePoints
                   .Select(remotePointConfiguration => new UndeterminedRemotePoint(remotePointConfiguration.Address, remotePointConfiguration.ListenPort))
                   .ToList(),
            configuration.StartupNodeSchemaLoad,
            configuration.StartupWebApi,
            configuration.PersistenceConnectionString,
            new InstanceManagement.WebApiOptions
            {
                ListenPort = configuration.WebApiConfiguration.ListenPort,
                ListenPortSecure = configuration.WebApiConfiguration.ListenPortSecure,
                UseSSL = configuration.WebApiConfiguration.UseSSL,
            }
            );

}
