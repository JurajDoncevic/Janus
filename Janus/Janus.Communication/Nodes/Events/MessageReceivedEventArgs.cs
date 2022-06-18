using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class MessageReceivedEventArgs<TMessage> : EventArgs where TMessage : BaseMessage
{
    private readonly TMessage _receivedMessage;
    private readonly RemotePoint _fromRemotePoint;

    public TMessage ReceivedMessage => _receivedMessage;

    public RemotePoint FromRemotePoint => _fromRemotePoint;

    public MessageReceivedEventArgs(TMessage receivedMessage, RemotePoint fromRemotePoint) : base()
    {
        _receivedMessage = receivedMessage;
        _fromRemotePoint = fromRemotePoint;
    }
}
