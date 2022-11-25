using Janus.Commons.Messages;
using Janus.Serialization.Bson.DataModels.DTOs;

namespace Janus.Serialization.Bson.Messages.DTOs;
internal sealed class QueryResMessageDto
{
    public new string Preamble { get => Preambles.QUERY_RESPONSE; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public TabularDataDto? TabularData { get; set; }
    public string ErrorMessage { get; set; }
    public int BlockNumber { get; set; }
    public int TotalBlocks { get; set; }
}
