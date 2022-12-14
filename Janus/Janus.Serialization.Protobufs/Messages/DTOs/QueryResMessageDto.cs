using Janus.Serialization.Protobufs.DataModels.DTOs;
using ProtoBuf;

namespace Janus.Serialization.Protobufs.Messages.DTOs;

[ProtoContract]
internal sealed class QueryResMessageDto
{
    [ProtoMember(1)]
    public string Preamble { get; set; }

    [ProtoMember(2)]
    public string ExchangeId { get; set; }

    [ProtoMember(3)]
    public string NodeId { get; set; }

    [ProtoMember(4)]
    public TabularDataDto? TabularData { get; set; }

    [ProtoMember(5)]
    public string OutcomeDescription { get; set; }

    [ProtoMember(6)]
    public int BlockNumber { get; set; }

    [ProtoMember(7)]
    public int TotalBlocks { get; set; }
}
