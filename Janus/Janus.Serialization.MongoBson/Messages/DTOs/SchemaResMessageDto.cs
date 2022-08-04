using Janus.Serialization.MongoBson.SchemaModels.DTOs;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal class SchemaResMessageDto : BaseMessageDto
{
    public DataSourceDto DataSource { get; set; }
}
