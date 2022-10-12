using ProtoBuf;

namespace Janus.Serialization.Protobufs.SchemaModels.DTOs;
/// <summary>
/// DTO representation of an UpdateSet
/// </summary>
[ProtoContract]
internal class UpdateSetDto
{
    [ProtoMember(1)]
    public HashSet<string> AttributeIds { get; set; } = new HashSet<string>();
}
