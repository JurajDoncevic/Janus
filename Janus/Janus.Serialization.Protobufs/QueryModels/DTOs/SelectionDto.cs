using Janus.Commons.SelectionExpressions;
using ProtoBuf;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Serialization.Protobufs.QueryModels.DTOs;

[ProtoContract]
internal class SelectionDto
{
    [ProtoMember(1)]
    public string Expression { get; set; } = "TRUE";
}
