using Janus.Commons.Messages;

namespace Janus.Serialization.Bson.Messages.DTOs;

internal class SchemaReqMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.SCHEMA_REQUEST; }
}