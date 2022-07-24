
using ProtoBuf;

namespace Janus.Serialization.Protobufs.SchemaModels.DTOs;

/// <summary>
/// /// <summary>
/// DTO representation of <see cref="SchemaModels"/>
/// </summary>
/// </summary>
[ProtoContract]
internal class SchemaDto
{
    [ProtoMember(1)]
    public string Name { get; set; }

    [ProtoMember(2)]
    public List<TableauDto> Tableaus { get; set; } = new List<TableauDto>();
}
