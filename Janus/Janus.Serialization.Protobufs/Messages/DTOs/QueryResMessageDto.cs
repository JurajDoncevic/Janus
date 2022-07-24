using Janus.Serialization.Protobufs.DataModels.DTOs;
using Janus.Serialization.Protobufs.QueryModels.DTOs;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Serialization.Protobufs.Messages.DTOs;

[ProtoContract]
internal class QueryResMessageDto
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
    public string ErrorMessage { get; set; }

    [ProtoMember(6)]
    public int BlockNumber { get; set; }

    [ProtoMember(7)]
    public int TotalBlocks { get; set; }
}
