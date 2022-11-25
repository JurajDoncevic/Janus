using Janus.Commons.Messages;
using Janus.Serialization.Bson.CommandModels.DTOs;

namespace Janus.Serialization.Bson.Messages.DTOs;
internal sealed class CommandReqMessageDto
{
    public string Preamble { get => Preambles.COMMAND_REQUEST; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public CommandReqTypes CommandReqType { get; set; }
    public DeleteCommandDto? DeleteCommandDto { get; set; } = null;
    public InsertCommandDto? InsertCommandDto { get; set; } = null;
    public UpdateCommandDto? UpdateCommandDto { get; set; } = null;
}
