using System.Text.Json.Serialization;

namespace Janus.Communication.Nodes;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NodeTypes
{
    MASK_NODE,
    MEDIATOR_NODE,
    WRAPPER_NODE
}
