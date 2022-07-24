using ProtoBuf;

namespace Janus.Serialization.Protobufs.QueryModels.DTOs;

[ProtoContract]
internal class ProjectionDto
{
    [ProtoMember(1)]
    public HashSet<string> AttributeIds { get; set; } = new();
}
