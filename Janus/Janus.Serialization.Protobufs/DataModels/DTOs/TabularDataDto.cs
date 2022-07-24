using Janus.Commons.SchemaModels;
using ProtoBuf;

namespace Janus.Serialization.Protobufs.DataModels.DTOs;

[ProtoContract]
internal class TabularDataDto
{
    [ProtoMember(1)]
    public Dictionary<string, DataTypes> AttributeDataTypes { get; set; } = new Dictionary<string, DataTypes>();

    [ProtoMember(2)]
    public List<RowValuesDto> AttributeValues { get; set; } = new List<RowValuesDto>();
}
