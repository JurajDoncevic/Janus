using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class ByeReqReceivedEventArgs : BaseMessageReceivedEventArgs<ByeReqMessage>
{
    public ByeReqReceivedEventArgs(ByeReqMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
