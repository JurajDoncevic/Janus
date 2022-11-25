using Janus.Serialization.Avro.SchemaModels.DTOs;

namespace Janus.Serialization.Avro.Messages.DTOs;
internal sealed class SchemaResMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public DataSourceDto DataSource { get; set; }
}
