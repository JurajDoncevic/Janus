using Janus.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.NetworkAdapters.Events;

public class QueryReqReceivedEventArgs : BaseMessageReceivedEventArgs<QueryReqMessage>
{
    public QueryReqReceivedEventArgs(QueryReqMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
