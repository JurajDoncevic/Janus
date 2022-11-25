
using ProtoBuf;

namespace Janus.Serialization.Protobufs.SchemaModels.DTOs;

/// <summary>
/// DTO representation of <see cref="Tableau"/>
/// </summary>
[ProtoContract]
internal sealed class TableauDto
{
    [ProtoMember(1)]
    public string Name { get; set; }

    [ProtoMember(2)]
    public string Description { get; set; }

    [ProtoMember(3)]
    public List<AttributeDto> Attributes { get; set; } = new List<AttributeDto>();

    [ProtoMember(4)]
    public HashSet<UpdateSetDto> UpdateSets { get; set; } = new HashSet<UpdateSetDto>();
}
