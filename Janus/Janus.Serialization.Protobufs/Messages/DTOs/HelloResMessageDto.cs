using Janus.Commons.Nodes;
using ProtoBuf;

namespace Janus.Serialization.Protobufs.Messages.DTOs;

[ProtoContract]
internal class HelloResMessageDto
{
    [ProtoMember(1)]
    public string Preamble { get; set; }

    [ProtoMember(2)]
    public string ExchangeId { get; set; }

    [ProtoMember(3)]
    public string NodeId { get; set; }

    [ProtoMember(4)]
    public int ListenPort { get; set; }

    [ProtoMember(5)]
    public NodeTypes NodeType { get; set; }

    [ProtoMember(6)]
    public bool RememberMe { get; set; }

    [ProtoMember(7)]
    public string ContextMessage { get; set; }
}