using Janus.Communication.Messages;
using Janus.Communication.Remotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Nodes.Events
{
    public class HelloReqEventArgs: EventArgs
    {
        private readonly HelloReqMessage _receivedMessage;
        private readonly RemotePoint _fromRemotePoint;

        public HelloReqMessage ReceivedMessage => _receivedMessage;

        public RemotePoint FromRemotePoint => _fromRemotePoint;

        public HelloReqEventArgs(HelloReqMessage receivedMessage, RemotePoint fromRemotePoint) : base()
        {
            _receivedMessage = receivedMessage;
            _fromRemotePoint = fromRemotePoint;
        }
    }
}
