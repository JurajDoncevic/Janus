using ProtoBuf;

namespace Janus.Serialization.Protobufs.DataModels.DTOs;

[ProtoContract]
internal sealed class RowValuesDto
{
    [ProtoMember(1)]
    public Dictionary<string, DataBytesDto> RowValues { get; set; }
}
