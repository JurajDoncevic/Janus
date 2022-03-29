using Janus.Communication.Messages;
using Janus.Communication.Remotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Nodes.Events
{
    public class ByeReqEventArgs : MessageReceivedEventArgs<ByeReqMessage>
    {
        public ByeReqEventArgs(ByeReqMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
        {
        }
    }
}
