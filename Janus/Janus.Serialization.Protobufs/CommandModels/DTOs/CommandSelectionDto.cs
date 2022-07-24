using ProtoBuf;

namespace Janus.Serialization.Protobufs.CommandModels.DTOs;

[ProtoContract]
public class CommandSelectionDto
{
    [ProtoMember(1)]
    public string SelectionExpression { get; set; } = "FALSE";
}
