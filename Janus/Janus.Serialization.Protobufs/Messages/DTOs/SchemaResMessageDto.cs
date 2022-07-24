using Janus.Serialization.Protobufs.SchemaModels.DTOs;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Serialization.Protobufs.Messages.DTOs;

[ProtoContract]
internal class SchemaResMessageDto
{
    [ProtoMember(1)]
    public string Preamble { get; set; }

    [ProtoMember(2)]
    public string ExchangeId { get; set; }

    [ProtoMember(3)]
    public string NodeId { get; set; }

    [ProtoMember(4)]
    public DataSourceDto DataSource { get; set; }
}
