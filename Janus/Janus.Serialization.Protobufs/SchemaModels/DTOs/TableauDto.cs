
using ProtoBuf;

namespace Janus.Serialization.Protobufs.SchemaModels.DTOs;

/// <summary>
/// DTO representation of <see cref="Tableau"/>
/// </summary>
[ProtoContract]
internal class TableauDto
{
    [ProtoMember(1)]
    public string Name { get; set; }

    [ProtoMember(2)]
    public List<AttributeDto> Attributes { get; set; } = new List<AttributeDto>();
}
