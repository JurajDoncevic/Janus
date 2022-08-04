using Janus.Serialization.Avro.QueryModels.DTOs;

namespace Janus.Serialization.Avro.Messages.DTOs;
internal class QueryReqMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public QueryDto Query { get; set; }
}
