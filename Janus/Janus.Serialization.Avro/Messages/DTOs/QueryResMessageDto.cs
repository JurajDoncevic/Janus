using Janus.Serialization.Avro.DataModels.DTOs;

namespace Janus.Serialization.Avro.Messages.DTOs;
internal sealed class QueryResMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public TabularDataDto? TabularData { get; set; }
    public string ErrorMessage { get; set; }
    public int BlockNumber { get; set; }
    public int TotalBlocks { get; set; }
}
