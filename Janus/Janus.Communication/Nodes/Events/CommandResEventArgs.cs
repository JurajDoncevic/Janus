using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Events;

public class CommandResEventArgs : MessageReceivedEventArgs<CommandResMessage>
{
    public CommandResEventArgs(CommandResMessage receivedMessage, RemotePoint fromRemotePoint) : base(receivedMessage, fromRemotePoint)
    {
    }
}
