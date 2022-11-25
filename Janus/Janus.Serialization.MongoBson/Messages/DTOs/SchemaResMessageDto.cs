using Janus.Serialization.MongoBson.SchemaModels.DTOs;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal sealed class SchemaResMessageDto : BaseMessageDto
{
    public DataSourceDto DataSource { get; set; }
}
