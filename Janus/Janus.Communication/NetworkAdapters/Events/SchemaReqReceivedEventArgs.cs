using Janus.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.NetworkAdapters.Events;

public class SchemaReqReceivedEventArgs : BaseMessageReceivedEventArgs<SchemaReqMessage>
{
    public SchemaReqReceivedEventArgs(SchemaReqMessage message, string senderAddress) : base(message, senderAddress)
    {
    }
}
