namespace Janus.Serialization.MongoBson.Messages.DTOs;

internal class CommandResMessageDto : BaseMessageDto
{
    public bool IsSuccess { get; set; } = false;
    public string OutcomeDescription { get; set; } = "";
}
