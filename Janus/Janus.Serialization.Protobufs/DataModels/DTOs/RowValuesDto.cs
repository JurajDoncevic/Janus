using ProtoBuf;

namespace Janus.Serialization.Protobufs.DataModels.DTOs;

[ProtoContract]
internal class RowValuesDto
{
    [ProtoMember(1)]
    public Dictionary<string, DataBytesDto> RowValues { get; set; }
}
