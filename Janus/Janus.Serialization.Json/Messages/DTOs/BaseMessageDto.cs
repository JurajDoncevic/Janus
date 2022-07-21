namespace Janus.Serialization.Json.Messages.DTOs;
internal abstract class BaseMessageDto
{
    public string Preamble { get; init; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
}
