using ProtoBuf;

namespace Janus.Serialization.Protobufs.DataModels.DTOs;

[ProtoContract]
internal sealed class DataBytesDto
{
    [ProtoMember(1)]
    public byte[] Data { get; set; }
}
