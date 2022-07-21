using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Mediator.Core;
using System.Text.Json.Serialization;

namespace Janus.Mediator.ConsoleApp;
public class MediatorConfigurationOptions
{
    public string NodeId { get; init; }

    public int ListenPort { get; init; }

    public int TimeoutMs { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CommunicationFormats CommunicationFormat { get; init; } = CommunicationFormats.AVRO;

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
            options.CommunicationFormat,
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