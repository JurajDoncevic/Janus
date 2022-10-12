
using ProtoBuf;

namespace Janus.Serialization.Protobufs.SchemaModels.DTOs;

/// <summary>
/// DTO representation of <see cref="DataSource"/>
/// </summary>
[ProtoContract]
internal class DataSourceDto
{
    [ProtoMember(1)]
    public string Name { get; set; }

    [ProtoMember(2)]
    public string Description { get; set; }

    [ProtoMember(3)]
    public string Version { get; set; }

    [ProtoMember(4)]
    public List<SchemaDto> Schemas { get; set; } = new List<SchemaDto>();
}
