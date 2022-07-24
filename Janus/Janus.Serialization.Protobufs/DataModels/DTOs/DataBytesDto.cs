using ProtoBuf;

namespace Janus.Serialization.Protobufs.DataModels.DTOs;

[ProtoContract]
internal class DataBytesDto
{
    [ProtoMember(1)]
    public byte[] Data { get; set; }
}
