using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class SchemaReqEventArgs : MessageReceivedEventArgs<SchemaReqMessage>
{
    public SchemaReqEventArgs(SchemaReqMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
    {
    }
}
