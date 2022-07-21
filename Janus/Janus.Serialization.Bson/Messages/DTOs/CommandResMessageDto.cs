using Janus.Commons.Messages;

namespace Janus.Serialization.Bson.Messages.DTOs;

internal class CommandResMessageDto
{
    public string Preamble { get => Preambles.COMMAND_RESPONSE; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public bool IsSuccess { get; set; } = false;
    public string OutcomeDescription { get; set; } = "";
}
