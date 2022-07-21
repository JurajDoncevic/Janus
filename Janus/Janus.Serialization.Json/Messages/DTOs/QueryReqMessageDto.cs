using Janus.Commons.Messages;
using Janus.Serialization.Json.QueryModels.DTOs;

namespace Janus.Serialization.Json.Messages.DTOs;
internal class QueryReqMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.QUERY_REQUEST; }
    public QueryDto Query { get; set; }
}
