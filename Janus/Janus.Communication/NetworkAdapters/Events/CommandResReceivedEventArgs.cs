using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class CommandResReceivedEventArgs : BaseMessageReceivedEventArgs<CommandResMessage>
{
    public CommandResReceivedEventArgs(CommandResMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
