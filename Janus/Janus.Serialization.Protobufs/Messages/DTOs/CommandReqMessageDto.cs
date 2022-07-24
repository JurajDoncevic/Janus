using Janus.Commons.Messages;
using Janus.Serialization.Protobufs.CommandModels.DTOs;
using ProtoBuf;

namespace Janus.Serialization.Protobufs.Messages.DTOs;

[ProtoContract]
internal class CommandReqMessageDto
{
    [ProtoMember(1)]
    public string Preamble { get; set; }

    [ProtoMember(2)]
    public string ExchangeId { get; set; }

    [ProtoMember(3)]
    public string NodeId { get; set; }

    [ProtoMember(4)]
    public CommandReqTypes CommandReqType { get; set; }

    [ProtoMember(5)]
    public DeleteCommandDto? DeleteCommandDto { get; set; } = null;

    [ProtoMember(6)]
    public InsertCommandDto? InsertCommandDto { get; set; } = null;

    [ProtoMember(7)]
    public UpdateCommandDto? UpdateCommandDto { get; set; } = null;
}
