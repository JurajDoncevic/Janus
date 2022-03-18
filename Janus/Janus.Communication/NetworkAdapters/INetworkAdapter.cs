using Janus.Communication.Messages;
using Janus.Communication.Remotes;

namespace Janus.Communication.NetworkAdapters;

public interface INetworkAdapter : IDisposable
{
    event EventHandler<HelloReceivedEventArgs> OnHelloMessageReceived;
    Result SendHelloMessage(HelloMessage message, RemotePoint remotePoint);
}
