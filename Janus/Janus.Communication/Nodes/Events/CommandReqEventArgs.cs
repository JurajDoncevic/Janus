using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class CommandReqEventArgs : MessageReceivedEventArgs<CommandReqMessage>
{
    public CommandReqEventArgs(CommandReqMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
    {
    }
}
