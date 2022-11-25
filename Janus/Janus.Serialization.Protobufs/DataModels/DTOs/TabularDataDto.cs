using Janus.Commons.SchemaModels;
using ProtoBuf;

namespace Janus.Serialization.Protobufs.DataModels.DTOs;

[ProtoContract]
internal sealed class TabularDataDto
{
    [ProtoMember(1)]
    public string Name { get; set; }

    [ProtoMember(2)]
    public Dictionary<string, DataTypes> AttributeDataTypes { get; set; } = new Dictionary<string, DataTypes>();

    [ProtoMember(3)]
    public List<RowValuesDto> AttributeValues { get; set; } = new List<RowValuesDto>();
}
