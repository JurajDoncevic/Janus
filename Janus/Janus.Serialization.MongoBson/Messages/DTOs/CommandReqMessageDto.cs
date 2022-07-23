using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.CommandModels.DTOs;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal class CommandReqMessageDto : BaseMessageDto
{
    public CommandReqTypes CommandReqType { get; set; }
    public DeleteCommandDto? DeleteCommandDto { get; set; } = null;
    public InsertCommandDto? InsertCommandDto { get; set; } = null;
    public UpdateCommandDto? UpdateCommandDto { get; set; } = null;
}
