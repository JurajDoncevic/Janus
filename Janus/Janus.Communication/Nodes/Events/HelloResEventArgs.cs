using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class HelloResEventArgs : MessageReceivedEventArgs<HelloResMessage>
{
    public HelloResEventArgs(HelloResMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
    {
    }
}
