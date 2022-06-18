using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class CommandReqReceivedEventArgs : BaseMessageReceivedEventArgs<CommandReqMessage>
{
    public CommandReqReceivedEventArgs(CommandReqMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
