using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Remotes;
using System.Collections.Concurrent;

namespace Janus.Communication.Nodes;

public abstract class CommunicationNode : IDisposable
{

    private readonly CommunicationNodeOptions _options;
    private readonly Dictionary<string, RemotePoint> _remotePoints;
    private readonly INetworkAdapter _networkAdapter;

    public CommunicationNodeOptions Options => _options;
    public ReadOnlyCollection<RemotePoint> RemotePoints => _remotePoints.Values.ToList().AsReadOnly();
    public abstract NodeTypes NodeType { get;}

    protected ConcurrentDictionary<string, BaseMessage> _receivedResponseMessages;
    protected ConcurrentDictionary<string, BaseMessage> _receivedRequestMessages;

    internal CommunicationNode(CommunicationNodeOptions options!!, INetworkAdapter networkAdapter)
    {
        _options = options;
        _remotePoints = new();
        _networkAdapter = networkAdapter;

        _receivedRequestMessages = new ConcurrentDictionary<string, BaseMessage>();
        _receivedResponseMessages = new ConcurrentDictionary<string, BaseMessage>();

        _networkAdapter.HelloRequestMessageReceived += ManageHelloRequest;
        _networkAdapter.HelloResponseMessageReceived += ManageHelloResponse;
    }

    private void ManageHelloResponse(object? sender, HelloResponseReceivedEventArgs e)
    {
        var message = e.Message;
        _receivedResponseMessages.AddOrUpdate(message.ExchangeId, message, (k, v) => message);
    }

    private void ManageHelloRequest(object? sender, HelloRequestReceivedEventArgs e)
    {
        // get message
        var message = e.Message;
        // create remote point
        var remotePoint = message.CreateRemotePoint(e.SenderAddress);
        // if remember me, save to remote points
        if (message.RememberMe)
        {
            _remotePoints.Add(message.NodeId, remotePoint);
        }
        // create response
        var response = new HelloResMessage(message.ExchangeId, _options.NodeId, _options.ListenPort, NodeType, message.RememberMe);
        // send response, but don't wait
        _networkAdapter.SendHelloResponse(response, remotePoint);
    }

    /// <summary>
    /// Sends a hello to the remote point and waits for a response. Doesn't save the remote point
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public async Task<DataResult<RemotePoint>> SendHello(RemotePoint remotePoint)
    {
        // create request hello message
        var helloRequest = new HelloReqMessage(_options.NodeId, _options.ListenPort, NodeType, false);
        var exchangdeId = helloRequest.ExchangeId;
        var result = Timing.RunWithTimeout(
            async () =>
                (await _networkAdapter.SendHelloRequest(helloRequest, remotePoint))
                    .Bind<RemotePoint>(_ =>
                    {
                        while (!_receivedResponseMessages.ContainsKey(exchangdeId)) ;
                        var helloResponse = (HelloResMessage)_receivedResponseMessages[exchangdeId];
                        var confirmedRemotePoint = helloResponse.CreateRemotePoint(remotePoint.Address);
                        return ResultExtensions.AsDataResult(() => confirmedRemotePoint);
                    }),
                _options.TimeoutMs);
        return await result;
    }

    /// <summary>
    /// Sends a hello to the remote point and waits for a response. Saves the remote point if hello is ok
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public async Task<DataResult<RemotePoint>> RegisterRemotePoint(RemotePoint remotePoint)
    {
        // create request hello message
        var helloRequest = new HelloReqMessage(_options.NodeId, _options.ListenPort, NodeType, true);
        var exchangdeId = helloRequest.ExchangeId;
        var result = Timing.RunWithTimeout(
            async () =>
                (await _networkAdapter.SendHelloRequest(helloRequest, remotePoint))
                    .Bind<RemotePoint>(_ =>
                    {
                        while (!_receivedResponseMessages.ContainsKey(exchangdeId)) ;
                        var helloResponse = (HelloResMessage)_receivedResponseMessages[exchangdeId];
                        var confirmedRemotePoint = helloResponse.CreateRemotePoint(remotePoint.Address);
                        return ResultExtensions.AsDataResult(() => confirmedRemotePoint);
                    }),
                _options.TimeoutMs);
        return await result;
    }
    public void Dispose()
    {
        _networkAdapter.Dispose();
    }
}

public static partial class CommunicationNodeExtensions
{
    public static RemotePoint CreateRemotePoint(this HelloResMessage helloResMessage!!, string senderAddress)
        => helloResMessage.NodeType switch
        {
            NodeTypes.MEDIATOR_NODE => new MediatorRemotePoint(helloResMessage.NodeId, senderAddress, helloResMessage.ListenPort),
            NodeTypes.WRAPPER_NODE => new WrapperRemotePoint(helloResMessage.NodeId, senderAddress, helloResMessage.ListenPort),
            NodeTypes.MASK_NODE => new MaskRemotePoint(helloResMessage.NodeId, senderAddress, helloResMessage.ListenPort)
        };
    public static RemotePoint CreateRemotePoint(this HelloReqMessage helloReqMessage!!, string senderAddress)
        => helloReqMessage.NodeType switch
        {
            NodeTypes.MEDIATOR_NODE => new MediatorRemotePoint(helloReqMessage.NodeId, senderAddress, helloReqMessage.ListenPort),
            NodeTypes.WRAPPER_NODE => new WrapperRemotePoint(helloReqMessage.NodeId, senderAddress, helloReqMessage.ListenPort),
            NodeTypes.MASK_NODE => new MaskRemotePoint(helloReqMessage.NodeId, senderAddress, helloReqMessage.ListenPort)
        };
}
