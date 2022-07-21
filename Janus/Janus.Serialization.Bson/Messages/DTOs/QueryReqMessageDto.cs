using Janus.Commons.Messages;
using Janus.Serialization.Bson.QueryModels.DTOs;

namespace Janus.Serialization.Bson.Messages.DTOs;
internal class QueryReqMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.QUERY_REQUEST; }
    public QueryDto Query { get; set; }
}
