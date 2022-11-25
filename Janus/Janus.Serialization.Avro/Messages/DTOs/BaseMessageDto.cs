namespace Janus.Serialization.Avro.Messages.DTOs;
public sealed class BaseMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
}
