﻿using Janus.Communication.Remotes;
using Janus.Mediator.Core;

namespace Janus.Mediator.Console;
public class MediatorConfigurationOptions
{
    public string NodeId { get; init; }

    public int ListenPort { get; init; }

    public int TimeoutMs { get; init; }

    public List<RemotePointOptions> StartupRemotePoints { get; init; } = new();
}

public enum NodeTypes
{
    MASK,
    MEDIATOR,
    WRAPPER
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
            options.StartupRemotePoints
                   .Select(remotePointOptions =>
                        (RemotePoint) (remotePointOptions switch
                        {
                            { NodeType: NodeTypes.MASK } rm => new MaskRemotePoint(rm.Address, rm.ListenPort),
                            { NodeType: NodeTypes.MEDIATOR } rm => new MediatorRemotePoint(rm.Address, rm.ListenPort),
                            { NodeType: NodeTypes.WRAPPER } rm => new WrapperRemotePoint(rm.Address, rm.ListenPort)
                        }))
                   .ToList()
            );

}