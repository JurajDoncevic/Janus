using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes;

public class HelloReceivedEventArgs : EventArgs
{
    private readonly HelloMessage _message;
    private readonly RemotePoint _remotePoint;
    public HelloMessage Message => _message;
    public RemotePoint RemotePoint => _remotePoint;

    public HelloReceivedEventArgs(HelloMessage message, RemotePoint remotePoint) : base()
    {
        _message = message;
        _remotePoint = remotePoint;
    }
}

