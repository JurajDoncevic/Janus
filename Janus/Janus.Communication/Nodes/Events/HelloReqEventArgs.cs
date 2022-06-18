using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class HelloReqEventArgs : MessageReceivedEventArgs<HelloReqMessage>
{
    public HelloReqEventArgs(HelloReqMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
    {
    }
}
