using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class HelloResReceivedEventArgs : BaseMessageReceivedEventArgs<HelloResMessage>
{
    public HelloResReceivedEventArgs(HelloResMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}

