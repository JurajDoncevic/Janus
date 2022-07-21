using Janus.Commons.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class QueryReqEventArgs : MessageReceivedEventArgs<QueryReqMessage>
{
    public QueryReqEventArgs(QueryReqMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
    {
    }
}
