using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class QueryReqReceivedEventArgs : BaseMessageReceivedEventArgs<QueryReqMessage>
{
    public QueryReqReceivedEventArgs(QueryReqMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
