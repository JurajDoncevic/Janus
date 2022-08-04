using Janus.Serialization.MongoBson.QueryModels.DTOs;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal class QueryReqMessageDto : BaseMessageDto
{
    public QueryDto Query { get; set; }
}
