using Janus.Commons.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class HelloReqEventArgs : MessageReceivedEventArgs<HelloReqMessage>
{
    private readonly bool _isRemotePointRegistered;
    public HelloReqEventArgs(HelloReqMessage receivedMessage, RemotePoint fromRemotePoint, bool isRemotePointRegistered) : base(receivedMessage, fromRemotePoint)
    {
        _isRemotePointRegistered = isRemotePointRegistered;
    }

    /// <summary>
    /// Has the remote point been registered on the hello request
    /// </summary>
    public bool IsRemotePointRegistered => _isRemotePointRegistered;
}
