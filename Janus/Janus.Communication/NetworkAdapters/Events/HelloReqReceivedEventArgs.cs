using Janus.Commons.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class HelloReqReceivedEventArgs : BaseMessageReceivedEventArgs<HelloReqMessage>
{
    public HelloReqReceivedEventArgs(HelloReqMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}

