
using ProtoBuf;

namespace Janus.Serialization.Protobufs.SchemaModels.DTOs;

/// <summary>
/// /// <summary>
/// DTO representation of <see cref="SchemaModels"/>
/// </summary>
/// </summary>
[ProtoContract]
internal sealed class SchemaDto
{
    [ProtoMember(1)]
    public string Name { get; set; }

    [ProtoMember(2)]
    public string Description { get; set; }

    [ProtoMember(3)]
    public List<TableauDto> Tableaus { get; set; } = new List<TableauDto>();
}
