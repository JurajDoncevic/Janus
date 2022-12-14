using Janus.Commons.Messages;
using Janus.Serialization.Bson.DataModels.DTOs;

namespace Janus.Serialization.Bson.Messages.DTOs;
internal sealed class QueryResMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.QUERY_RESPONSE; }
    public TabularDataDto? TabularData { get; set; }
    public string OutcomeDescription { get; set; }
    public int BlockNumber { get; set; }
    public int TotalBlocks { get; set; }
}
