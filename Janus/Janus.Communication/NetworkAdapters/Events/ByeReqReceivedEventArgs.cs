using Janus.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.NetworkAdapters.Events;

public class ByeReqReceivedEventArgs : BaseMessageReceivedEventArgs<ByeReqMessage>
{
    public ByeReqReceivedEventArgs(ByeReqMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
