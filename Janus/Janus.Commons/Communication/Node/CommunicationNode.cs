using Janus.Commons.Communication.Messages;
using Janus.Commons.Communication.Remotes;
using System.Net.Sockets;

namespace Janus.Commons.Communication.Node;

public abstract class CommunicationNode
{

    private readonly CommunicationNodeOptions _options;
    private readonly TcpClient _tcpClient;
    private readonly List<RemotePoint> _remotePoints;

    public CommunicationNodeOptions Options => _options;
    public ReadOnlyCollection<RemotePoint> RemotePoints => _remotePoints.AsReadOnly();

    public CommunicationNode(CommunicationNodeOptions options!!, IEnumerable<RemotePoint> remotePoints!!)
    {
        _options = options;
        _remotePoints = remotePoints.ToList();
    }

    public CommunicationNode(CommunicationNodeOptions options!!)
    {
        _options = options;
        _remotePoints = new();
    }

    public abstract Result RegisterRemotePoint();
    public abstract Result UnregisterRemotePoint();

    public abstract DataResult<RemotePoint> SendHello(string Ip);

    protected abstract DataResult<TResult> SendMessage<TResult>(IMessage message, RemotePoint remotePoint);
}
