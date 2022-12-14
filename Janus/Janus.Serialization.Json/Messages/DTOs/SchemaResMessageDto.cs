using Janus.Commons.Messages;
using Janus.Serialization.Json.SchemaModels.DTOs;

namespace Janus.Serialization.Json.Messages.DTOs;
internal class SchemaResMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.SCHEMA_RESPONSE; }
    public DataSourceDto? DataSource { get; set; }
    public string OutcomeDescription { get; set; }
}
