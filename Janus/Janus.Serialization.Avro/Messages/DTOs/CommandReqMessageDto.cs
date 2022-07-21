using Janus.Commons.Messages;
using Janus.Serialization.Avro.CommandModels.DTOs;

namespace Janus.Serialization.Avro.Messages.DTOs;
internal class CommandReqMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public CommandReqTypes CommandReqType { get; set; }
    public DeleteCommandDto? DeleteCommandDto { get; set; } = null;
    public InsertCommandDto? InsertCommandDto { get; set; } = null;
    public UpdateCommandDto? UpdateCommandDto { get; set; } = null;
}
