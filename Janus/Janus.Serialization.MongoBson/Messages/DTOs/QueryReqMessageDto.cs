using Janus.Serialization.MongoBson.QueryModels.DTOs;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal sealed class QueryReqMessageDto : BaseMessageDto
{
    public QueryDto Query { get; set; }
}
