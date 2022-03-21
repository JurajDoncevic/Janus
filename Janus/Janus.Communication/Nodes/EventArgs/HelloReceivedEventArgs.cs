using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes;

public class HelloReceivedEventArgs : EventArgs
{
    private readonly HelloReqMessage _message;
    private readonly RemotePoint _remotePoint;
    public HelloReqMessage Message => _message;
    public RemotePoint RemotePoint => _remotePoint;

    public HelloReceivedEventArgs(HelloReqMessage message, RemotePoint remotePoint) : base()
    {
        _message = message;
        _remotePoint = remotePoint;
    }
}

