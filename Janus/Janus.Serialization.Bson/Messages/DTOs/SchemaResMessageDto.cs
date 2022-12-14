using Janus.Commons.Messages;
using Janus.Serialization.Bson.SchemaModels.DTOs;

namespace Janus.Serialization.Bson.Messages.DTOs;
internal sealed class SchemaResMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.SCHEMA_RESPONSE; }
    public DataSourceDto? DataSource { get; set; }
    public string OutcomeDescription { get; set; }
}
