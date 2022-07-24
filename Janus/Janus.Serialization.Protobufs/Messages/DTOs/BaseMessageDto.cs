using ProtoBuf;

namespace Janus.Serialization.Protobufs.Messages.DTOs;

[ProtoContract]
public class BaseMessageDto
{
    [ProtoMember(1)]
    public string Preamble { get; set; }

    [ProtoMember(2)]
    public string ExchangeId { get; set; }

    [ProtoMember(3)]
    public string NodeId { get; set; }
}
