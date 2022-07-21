using Janus.Commons.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class ByeReqEventArgs : MessageReceivedEventArgs<ByeReqMessage>
{
    public ByeReqEventArgs(ByeReqMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
    {
    }
}
