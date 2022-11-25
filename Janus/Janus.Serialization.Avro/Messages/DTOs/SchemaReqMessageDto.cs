namespace Janus.Serialization.Avro.Messages.DTOs;

internal sealed class SchemaReqMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
}