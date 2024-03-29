﻿using Janus.Commons.Messages;
using Janus.Commons.Nodes;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Nodes.Utils;
using Janus.Communication.Remotes;
using Janus.Logging;

namespace Janus.Communication.Nodes.Implementations;

public abstract class BaseCommunicationNode<TNetworkAdapter> : IDisposable, ICommunicationNode where TNetworkAdapter : INetworkAdapter
{

    protected readonly CommunicationNodeOptions _options;
    protected readonly Dictionary<string, RemotePoint> _remotePoints;
    protected readonly TNetworkAdapter _networkAdapter;

    /// <summary>
    /// Communication node options
    /// </summary>
    public CommunicationNodeOptions Options => _options;

    /// <summary>
    /// Registered remote points (currently active)
    /// </summary>
    public ReadOnlyCollection<RemotePoint> RemotePoints => _remotePoints.Values.ToList().AsReadOnly();

    /// <summary>
    /// This node's type
    /// </summary>
    public abstract NodeTypes NodeType { get; }

    protected readonly MessageStore _messageStore;

    private readonly ILogger<BaseCommunicationNode<TNetworkAdapter>>? _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="options">Communication node options</param>
    /// <param name="networkAdapter">Node's network adapter</param>
    protected BaseCommunicationNode(CommunicationNodeOptions options, TNetworkAdapter networkAdapter, ILogger? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _remotePoints = new();
        _networkAdapter = networkAdapter;

        _logger = logger?.ResolveLogger<BaseCommunicationNode<TNetworkAdapter>>();

        _messageStore = new MessageStore();

        _networkAdapter.HelloRequestReceived += NetworkAdapter_ManageHelloRequestReceived;
        _networkAdapter.HelloResponseReceived += NetworkAdapter_ManageHelloResponseReceived;
        _networkAdapter.ByeRequestReceived += NetworkAdapter_ManageByeRequestReceived;

        _networkAdapter.StartAdapter();
        _logger?.Info("Initialized communication {0} node {1} on port {2}", NodeType, _options.NodeId, _options.ListenPort);
    }

    #region RECEIVED MESSAGE EVENTS
    public event EventHandler<HelloReqEventArgs>? HelloRequestReceived;
    public event EventHandler<HelloResEventArgs>? HelloResponseReceived;
    public event EventHandler<ByeReqEventArgs>? ByeRequestReceived;
    #endregion

    #region MANAGE INCOMING MESSAGES

    /// <summary>
    /// Manage a BYE_REQ message
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NetworkAdapter_ManageByeRequestReceived(object? sender, ByeReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info("Managing {0} from node {1}", message.Preamble, message.NodeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];
            // remove the remote point
            _remotePoints.Remove(message.NodeId);
            _logger?.Info("Removed node {0} from registered remote points on {1}.", message.NodeId, message.Preamble);

            // invoke event
            ByeRequestReceived?.Invoke(this, new ByeReqEventArgs(e.Message, remotePoint));
        }
    }

    /// <summary>
    /// Manages a HELLO_RES message
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NetworkAdapter_ManageHelloResponseReceived(object? sender, HelloResReceivedEventArgs e)
    {
        // extract the message
        var message = e.Message;
        _logger?.Info("Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);

        // add the message to the responses dictionary
        var enqueued = _messageStore.EnqueueResponseInExchange(message.ExchangeId, message);

        if (enqueued)
        {
            _logger?.Info("Added {0} from {1} in exchange {2} to received responses", message.Preamble, message.NodeId, message.ExchangeId);

            //invoke event
            HelloResponseReceived?.Invoke(this, new HelloResEventArgs(message, _remotePoints[message.NodeId]));
        }
        else
        {
            _logger?.Info("Unknown exchange {0} for {1}", message.ExchangeId, message.Preamble);
        }

    }

    /// <summary>
    /// Manages a HELLO_REQ message. HELLO_REQ is not added to the request dictionary
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NetworkAdapter_ManageHelloRequestReceived(object? sender, HelloReqReceivedEventArgs e)
    {
        // get message
        var message = e.Message;
        _logger?.Info("Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);

        // declare response
        HelloResMessage response = null;

        // create remote point
        var remotePoint = message.CreateRemotePoint(e.SenderAddress);

        // this is a register scenario, see if you should save to remote points
        if (message.RememberMe)
        {
            var isOkToRegister = IsRemotePointOkToRegister(remotePoint);
            if (!isOkToRegister)
            {
                // create negative response
                response = new HelloResMessage(message.ExchangeId, _options.NodeId, _options.ListenPort, NodeType, isOkToRegister, isOkToRegister.Message);
                _logger?.Info("Refused to register remote point {0} on request: {1}", remotePoint, isOkToRegister.Message);
            }
            else // this remote point is ok to register
            {
                _remotePoints[message.NodeId] = remotePoint;
                _logger?.Info("Registered remote point {0}", remotePoint);
                response = new HelloResMessage(message.ExchangeId, _options.NodeId, _options.ListenPort, NodeType, message.RememberMe);
            }
        }
        else // this is a simple HELLO ping scenario
        {
            response = new HelloResMessage(message.ExchangeId, _options.NodeId, _options.ListenPort, NodeType, message.RememberMe);
        }

        // send response, but don't wait
        _logger?.Info("Sending {0} as a response to remote point {1} in exchange {2}", response.Preamble, remotePoint, response.ExchangeId);
        _networkAdapter.SendHelloResponse(response, remotePoint);

        // invoke event
        HelloRequestReceived?.Invoke(this, new HelloReqEventArgs(message, remotePoint, response.RememberMe));
    }

    #endregion

    #region SEND MESSAGES
    /// <summary>
    /// Sends a hello to the remote point and waits for a response. Doesn't save the remote point
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public async Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
    {
        // create request hello message
        var helloRequest = new HelloReqMessage(_options.NodeId, _options.ListenPort, NodeType, false);
        var exchangeId = helloRequest.ExchangeId;

        // register the exchange so a response can be received
        _messageStore.RegisterExchange(exchangeId);

        var result = await Timing.RunWithTimeout(
            async (token) =>
                (await _networkAdapter.SendHelloRequest(helloRequest, remotePoint))
                    .Pass(
                        result => _logger?.Info("Sending {0} to {1} successful with exchange {2}", helloRequest.Preamble, remotePoint, helloRequest.ExchangeId),
                        result => _logger?.Info("Sending {0} to {1} failed with message {2}", helloRequest.Preamble, remotePoint, result.Message)
                    )
                    .Bind(result => Results.AsResult(() =>
                    {
                        // wait for the response to appear
                        while (!_messageStore.AnyResponsesExist(exchangeId))
                        {
                            if (token.IsCancellationRequested)
                            {
                                _logger?.Info("Timeout reached waiting for {0} response in exchange {1}", helloRequest.Preamble, helloRequest.ExchangeId);
                                token.ThrowIfCancellationRequested();
                            }
                        }
                        // get the hello response - exception if not correct message type or incorrectly dequeued
                        var helloResponse = (HelloResMessage)_messageStore.DequeueResponseFromExchange(exchangeId).Data;
                        _logger?.Info("Received returned {0} from {1} in exchange {2}", helloResponse.Preamble, helloResponse.NodeId, helloResponse.ExchangeId);

                        // create a remote point from the message and sender address
                        var confirmedRemotePoint = helloResponse.CreateRemotePoint(remotePoint.Address);

                        // turn it into a data result
                        return confirmedRemotePoint;
                    })),
                _options.TimeoutMs);

        // unregister the exchange
        _messageStore.UnregisterExchange(exchangeId);

        return result;
    }

    /// <summary>
    /// Sends a hello to the remote point and waits for a response. Saves the remote point if hello is ok
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public async Task<Result<RemotePoint>> RegisterRemotePoint(UndeterminedRemotePoint remotePoint)
    {
        // create request hello message
        var helloRequest = new HelloReqMessage(_options.NodeId, _options.ListenPort, NodeType, true);
        var exchangeId = helloRequest.ExchangeId;

        // register the exchange so a response can be received
        _messageStore.RegisterExchange(exchangeId);

        var result = await Timing.RunWithTimeout(
            async (token) =>
                (await _networkAdapter.SendHelloRequest(helloRequest, remotePoint))
                    .Pass(
                        result => _logger?.Info("Sending {0} with registering intention to {1} with exchange {2} successful.", helloRequest.Preamble, remotePoint, helloRequest.ExchangeId),
                        result => _logger?.Info("Sending {0} with registering intention to {1} failed with message {2}", helloRequest.Preamble, remotePoint, result.Message)
                    )
                    .Bind(result => Results.AsResult(() =>
                    {
                        // wait for the response
                        while (!_messageStore.AnyResponsesExist(exchangeId))
                        {
                            if (token.IsCancellationRequested)
                            {
                                _logger?.Info("Timeout reached waiting for {0} response in exchange {1}", helloRequest.Preamble, helloRequest.ExchangeId);
                                token.ThrowIfCancellationRequested();
                            }
                        }
                        // get the response
                        var helloResponse = (HelloResMessage)_messageStore.DequeueResponseFromExchange(exchangeId).Data;
                        _logger?.Info("Received {0} on exchange {1} from {2}. Registering remote point.", helloResponse.Preamble, helloResponse.ExchangeId, helloResponse.NodeId);

                        // create a remote point from the message and sender address
                        var respondingRemotePoint = helloResponse.CreateRemotePoint(remotePoint.Address);
                        // don't register the node if it refused registration
                        if (!helloResponse.RememberMe)
                        {
                            _logger?.Info("Remote point {0} refused register. Reason: {1}", respondingRemotePoint, helloResponse.ContextMessage);
                            return Results.OnFailure<RemotePoint>($"Remote point {respondingRemotePoint} refused register. Reason: {helloResponse.ContextMessage}");
                        }
                        // add update the remote point into the known remote point dictionary
                        _remotePoints[respondingRemotePoint.NodeId] = respondingRemotePoint;
                        // turn the remote point into a data result
                        return respondingRemotePoint;
                    })),
                _options.TimeoutMs);

        // unregister the exchange
        _messageStore.UnregisterExchange(exchangeId);

        return result;
    }

    /// <summary>
    /// Sends a bye to the remote point if it exists in the saved remote points. Removes the remote point from the saved remote points.
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public async Task<Result> SendBye(RemotePoint remotePoint)
    {
        if (!_remotePoints.ContainsValue(remotePoint))
            return Results.AsResult(() => false);

        var message = new ByeReqMessage(_options.NodeId);

        var result = (await Timing.RunWithTimeout(
            async token => (await _networkAdapter.SendByeRequest(message, remotePoint).WaitAsync(token))
                                .Pass(r => _logger?.Info("Sent {0} to {1}", message.Preamble, remotePoint),
                                      r => _logger?.Info("Sending {0} to {1} failed with message {2}", message.Preamble, remotePoint, r.Message)),
            _options.TimeoutMs))
            .Pass(
                r => _remotePoints.Remove(remotePoint.NodeId)
                        ? _logger?.Info("Removed remote point {0}", remotePoint) ?? Unit()
                        : Unit(),
                r => Unit());

        return result;
    }

    #endregion

    /// <summary>
    /// Determines if the requesting remote point should be registered
    /// </summary>
    /// <param name="remotePoint">Remote point requesting registration</param>
    /// <returns>Result with message of possible refusal reason</returns>
    protected abstract Result IsRemotePointOkToRegister(RemotePoint remotePoint);

    public void Dispose()
    {
        _networkAdapter.Dispose();
        _logger?.Info("Disposed communication {0} node {1} on port {2}", NodeType, _options.NodeId, _options.ListenPort);
    }
}

public static partial class CommunicationNodeExtensions
{
    /// <summary>
    /// Creates a remote point from response and address data 
    /// </summary>
    /// <param name="helloResMessage">HELLO_RES message</param>
    /// <param name="senderAddress">Sender address of the HELLO_RES message</param>
    /// <returns></returns>
    public static RemotePoint CreateRemotePoint(this HelloResMessage helloResMessage, string senderAddress)
    {
        if (helloResMessage is null)
        {
            throw new ArgumentNullException(nameof(helloResMessage));
        }

        return helloResMessage.NodeType switch
        {
            NodeTypes.MEDIATOR => new MediatorRemotePoint(helloResMessage.NodeId, senderAddress, helloResMessage.ListenPort),
            NodeTypes.WRAPPER => new WrapperRemotePoint(helloResMessage.NodeId, senderAddress, helloResMessage.ListenPort),
            NodeTypes.MASK => new MaskRemotePoint(helloResMessage.NodeId, senderAddress, helloResMessage.ListenPort)
        };
    }

    /// <summary>
    /// Creates a remote point from request and address data 
    /// </summary>
    /// <param name="helloResMessage">HELLO_REQ message</param>
    /// <param name="senderAddress">Sender address of the HELLO_REQ message</param>
    /// <returns></returns>
    public static RemotePoint CreateRemotePoint(this HelloReqMessage helloReqMessage, string senderAddress)
    {
        if (helloReqMessage is null)
        {
            throw new ArgumentNullException(nameof(helloReqMessage));
        }

        return helloReqMessage.NodeType switch
        {
            NodeTypes.MEDIATOR => new MediatorRemotePoint(helloReqMessage.NodeId, senderAddress, helloReqMessage.ListenPort),
            NodeTypes.WRAPPER => new WrapperRemotePoint(helloReqMessage.NodeId, senderAddress, helloReqMessage.ListenPort),
            NodeTypes.MASK => new MaskRemotePoint(helloReqMessage.NodeId, senderAddress, helloReqMessage.ListenPort)
        };
    }
}
