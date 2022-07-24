using ProtoBuf;

namespace Janus.Serialization.Protobufs.QueryModels.DTOs;

[ProtoContract]
internal class JoinDto
{
    [ProtoMember(1)]
    public string PrimaryKeyAttributeId { get; set; } = string.Empty;

    [ProtoMember(2)]
    public string PrimaryKeyTableauId { get; set; } = string.Empty;

    [ProtoMember(3)]
    public string ForeignKeyAttributeId { get; set; } = string.Empty;

    [ProtoMember(4)]
    public string ForeignKeyTableauId { get; set; } = string.Empty;
}
