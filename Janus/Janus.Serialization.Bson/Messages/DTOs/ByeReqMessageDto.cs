using Janus.Commons.Messages;

namespace Janus.Serialization.Bson.Messages.DTOs;
internal sealed class ByeReqMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.BYE_REQUEST; }
}
