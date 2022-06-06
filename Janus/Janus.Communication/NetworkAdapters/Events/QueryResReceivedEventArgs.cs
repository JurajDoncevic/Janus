using Janus.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.NetworkAdapters.Events;

public class QueryResReceivedEventArgs : BaseMessageReceivedEventArgs<QueryResMessage>
{
    public QueryResReceivedEventArgs(QueryResMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
