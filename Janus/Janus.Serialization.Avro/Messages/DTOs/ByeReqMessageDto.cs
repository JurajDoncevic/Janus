namespace Janus.Serialization.Avro.Messages.DTOs;

internal class ByeReqMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
}