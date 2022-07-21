namespace Janus.Serialization.Avro.Messages.DTOs;

internal class CommandResMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public bool IsSuccess { get; set; } = false;
    public string OutcomeDescription { get; set; } = "";
}
