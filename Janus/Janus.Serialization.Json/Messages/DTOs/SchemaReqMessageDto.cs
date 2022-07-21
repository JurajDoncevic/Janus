using Janus.Commons.Messages;

namespace Janus.Serialization.Json.Messages.DTOs;

internal class SchemaReqMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.SCHEMA_REQUEST; }
}