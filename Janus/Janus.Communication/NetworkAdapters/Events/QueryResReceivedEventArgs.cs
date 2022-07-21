using Janus.Commons.Messages;

namespace Janus.Communication.NetworkAdapters.Events;

public class QueryResReceivedEventArgs : BaseMessageReceivedEventArgs<QueryResMessage>
{
    public QueryResReceivedEventArgs(QueryResMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
