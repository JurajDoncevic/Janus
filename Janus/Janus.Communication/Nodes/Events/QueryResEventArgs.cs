using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class QueryResEventArgs : MessageReceivedEventArgs<QueryResMessage>
{
    public QueryResEventArgs(QueryResMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
    {
    }
}
