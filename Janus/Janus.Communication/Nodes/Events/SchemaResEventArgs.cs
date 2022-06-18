using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class SchemaResEventArgs : MessageReceivedEventArgs<SchemaResMessage>
{
    public SchemaResEventArgs(SchemaResMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
    {
    }
}
