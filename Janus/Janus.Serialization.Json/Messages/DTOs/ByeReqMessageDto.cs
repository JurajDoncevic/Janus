using Janus.Commons.Messages;

namespace Janus.Serialization.Json.Messages.DTOs;
internal class ByeReqMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.BYE_REQUEST; }
}
