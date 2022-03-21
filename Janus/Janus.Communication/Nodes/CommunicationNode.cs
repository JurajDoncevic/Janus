using Janus.Communication.Messages;
using NetworkAdapters = Janus.Communication.NetworkAdapters;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes;

public abstract class CommunicationNode : IDisposable
{

    private readonly CommunicationNodeOptions _options;
    private readonly Dictionary<string, RemotePoint> _remotePoints;
    private readonly NetworkAdapters.INetworkAdapter _networkAdapter;
    private readonly List<string> _awaitingHelloResponses;

    public CommunicationNodeOptions Options => _options;
    public ReadOnlyCollection<RemotePoint> RemotePoints => _remotePoints.Values.ToList().AsReadOnly();
    public abstract NodeTypes NodeType { get;}

    public event EventHandler<HelloReceivedEventArgs> OnHelloReceived;

    internal CommunicationNode(CommunicationNodeOptions options!!, NetworkAdapters.INetworkAdapter networkAdapter)
    {
        _options = options;
        _remotePoints = new();
        _networkAdapter = networkAdapter;
        _awaitingHelloResponses = new ();

        _networkAdapter.OnHelloMessageReceived += HelloReceivedFromAdapter;
    }

    protected void HelloReceivedFromAdapter(object? sender, NetworkAdapters.HelloReceivedEventArgs e)
    {
        var message = e.Message;
        if (_awaitingHelloResponses.Contains(message.ExchangeId))
        {
            // this is a response to an already sent hello message
            // remove from wait list
            _awaitingHelloResponses.Remove(message.ExchangeId);
            // create remote point
            var remotePoint = e.CreateRemotePoint();
            // add to remotes list
            _remotePoints.Add(remotePoint.Id, remotePoint);
        }
        else
        {
            // this is a newly received hello message
            // create remote point
            var remotePoint = e.CreateRemotePoint();
            // add to remotes list
            _remotePoints.Add(remotePoint.Id, remotePoint);
            // send response
            SendHelloResponse(message.ExchangeId, remotePoint);
        }
        OnHelloReceived?.Invoke(this, new HelloReceivedEventArgs(message, e.CreateRemotePoint()));
    }

    public Result SendHello(RemotePoint remotePoint)
    {
        // create hello
        var message = new HelloReqMessage(_options.Id, _options.Port, NodeType);
        // add exhange id to awaited responses
        _awaitingHelloResponses.Add(message.ExchangeId);
        // send hello
        return _networkAdapter.SendHelloMessage(message, remotePoint);
    }

    protected Result SendHelloResponse(string exchangeId, RemotePoint remotePoint)
    {
        // create hello
        var message = new HelloReqMessage(exchangeId, _options.Id, _options.Port, NodeType);
        // send hello
        return _networkAdapter.SendHelloMessage(message, remotePoint);
    }

    public void Dispose()
    {
        _networkAdapter.Dispose();
    }
}

public static partial class CommunicationNodeExtensions
{
    public static RemotePoint CreateRemotePoint(this NetworkAdapters.HelloReceivedEventArgs eventArgs)
        => eventArgs.Message.NodeType switch
        {
            NodeTypes.MEDIATOR_NODE => new MediatorRemotePoint(eventArgs.Message.NodeId, eventArgs.SenderAddress, eventArgs.Message.ListenPort),
            NodeTypes.WRAPPER_NODE => new WrapperRemotePoint(eventArgs.Message.NodeId, eventArgs.SenderAddress, eventArgs.Message.ListenPort),
            NodeTypes.MASK_NODE => new MaskRemotePoint(eventArgs.Message.NodeId, eventArgs.SenderAddress, eventArgs.Message.ListenPort)
        };
}
