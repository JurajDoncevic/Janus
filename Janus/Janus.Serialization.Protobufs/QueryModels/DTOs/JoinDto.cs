using ProtoBuf;

namespace Janus.Serialization.Protobufs.QueryModels.DTOs;

[ProtoContract]
internal sealed class JoinDto
{
    [ProtoMember(1)]
    public string PrimaryKeyAttributeId { get; set; } = string.Empty;

    [ProtoMember(2)]
    public string ForeignKeyAttributeId { get; set; } = string.Empty;

}
