﻿using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Nodes.Events;
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

    #region RECEIVED MESSAGE EVENTS
    public event EventHandler<HelloReqEventArgs> HelloRequestReceived;
    public event EventHandler<ByeReqEventArgs> ByeRequestReceived;
    #endregion

    internal protected CommunicationNode(CommunicationNodeOptions options!!, INetworkAdapter networkAdapter)
    {
        _options = options;
        _remotePoints = new();
        _networkAdapter = networkAdapter;

        _receivedRequestMessages = new ConcurrentDictionary<string, BaseMessage>();
        _receivedResponseMessages = new ConcurrentDictionary<string, BaseMessage>();

        _networkAdapter.HelloRequestReceived += ManageHelloRequest;
        _networkAdapter.HelloResponseReceived += ManageHelloResponse;
        _networkAdapter.ByeRequestReceived += ManageByeRequest;
    }

    #region MANAGE INCOMING MESSAGES

    /// <summary>
    /// Manage a BYE_REQ message
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ManageByeRequest(object? sender, ByeReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];
            // raise event
            ByeRequestReceived?.Invoke(this, new ByeReqEventArgs(e.Message, remotePoint));
            // remove the remote point
            _remotePoints.Remove(message.NodeId);
        }
    }

    /// <summary>
    /// Manages a HELLO_RES message
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ManageHelloResponse(object? sender, HelloResReceivedEventArgs e)
    {
        // extract the message
        var message = e.Message;
        // add the message to the responses dictionary
        _receivedResponseMessages.AddOrUpdate(message.ExchangeId, message, (k, v) => message);
    }

    /// <summary>
    /// Manages a HELLO_REQ message. HELLO_REQ is not added to the request dictionary
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ManageHelloRequest(object? sender, HelloReqReceivedEventArgs e)
    {
        // get message
        var message = e.Message;
        // create remote point
        var remotePoint = message.CreateRemotePoint(e.SenderAddress);
        // raise event
        HelloRequestReceived?.Invoke(this, new HelloReqEventArgs(message, remotePoint));
        // if remember me, save to remote points
        if (message.RememberMe)
        {
            _remotePoints[message.NodeId] = remotePoint;
        }
        // create response
        var response = new HelloResMessage(message.ExchangeId, _options.NodeId, _options.ListenPort, NodeType, message.RememberMe);
        // send response, but don't wait
        _networkAdapter.SendHelloResponse(response, remotePoint);
    }

    #endregion

    #region SEND MESSAGES
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
            async (token) =>
                (await _networkAdapter.SendHelloRequest(helloRequest, remotePoint))
                    .Bind<RemotePoint>(result =>
                    {
                        // wait for the response to appear
                        while (!_receivedResponseMessages.ContainsKey(exchangdeId))
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        // get the hello response - exception if not correct message type
                        var helloResponse = (HelloResMessage)_receivedResponseMessages[exchangdeId];
                        // remove the response from the concurrent dict
                        _receivedResponseMessages.Remove(exchangdeId, out _);
                        // create a remote point from the message and sender address
                        var confirmedRemotePoint = helloResponse.CreateRemotePoint(remotePoint.Address);
                        // turn it into a data result
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
            async (token) =>
                (await _networkAdapter.SendHelloRequest(helloRequest, remotePoint))
                    .Bind<RemotePoint>(result =>
                    {
                        // wait for the response
                        while (!_receivedResponseMessages.ContainsKey(exchangdeId))
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        // get the response
                        var helloResponse = (HelloResMessage)_receivedResponseMessages[exchangdeId];
                        // remove the response from the dictionary
                        _receivedResponseMessages.Remove(exchangdeId, out _);
                        // create a remote point from the message and sender address
                        var confirmedRemotePoint = helloResponse.CreateRemotePoint(remotePoint.Address);
                        // add update the remote point into the known remote point dictionary
                        _remotePoints[confirmedRemotePoint.NodeId] = confirmedRemotePoint;
                        // turn the remote point into a data result
                        return ResultExtensions.AsDataResult(() => confirmedRemotePoint);
                    }),
                _options.TimeoutMs);
        return await result;
    }

    /// <summary>
    /// Sends a bye to the remote point if it exists in the saved remote points. Removes the remote point from the saved remote points.
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public async Task<Result> SendBye(RemotePoint remotePoint)
    {
        if (!_remotePoints.ContainsValue(remotePoint))
            return ResultExtensions.AsResult(() => false);

        var message = new ByeReqMessage(_options.NodeId);

        var result = (await Timing.RunWithTimeout(
            async token =>
            {
                return await _networkAdapter.SendByeRequest(message, remotePoint);
            },
            _options.TimeoutMs))
            .Pass(r =>
            {
                if (r.IsSuccess)
                {
                    _remotePoints.Remove(remotePoint.NodeId);
                }
                return Unit();
            });
            
        return result;
    }

    #endregion

    public void Dispose()
    {
        _networkAdapter.Dispose();
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
    public static RemotePoint CreateRemotePoint(this HelloResMessage helloResMessage!!, string senderAddress)
        => helloResMessage.NodeType switch
        {
            NodeTypes.MEDIATOR_NODE => new MediatorRemotePoint(helloResMessage.NodeId, senderAddress, helloResMessage.ListenPort),
            NodeTypes.WRAPPER_NODE => new WrapperRemotePoint(helloResMessage.NodeId, senderAddress, helloResMessage.ListenPort),
            NodeTypes.MASK_NODE => new MaskRemotePoint(helloResMessage.NodeId, senderAddress, helloResMessage.ListenPort)
        };
    /// <summary>
    /// Creates a remote point from request and address data 
    /// </summary>
    /// <param name="helloResMessage">HELLO_REQ message</param>
    /// <param name="senderAddress">Sender address of the HELLO_REQ message</param>
    /// <returns></returns>
    public static RemotePoint CreateRemotePoint(this HelloReqMessage helloReqMessage!!, string senderAddress)
        => helloReqMessage.NodeType switch
        {
            NodeTypes.MEDIATOR_NODE => new MediatorRemotePoint(helloReqMessage.NodeId, senderAddress, helloReqMessage.ListenPort),
            NodeTypes.WRAPPER_NODE => new WrapperRemotePoint(helloReqMessage.NodeId, senderAddress, helloReqMessage.ListenPort),
            NodeTypes.MASK_NODE => new MaskRemotePoint(helloReqMessage.NodeId, senderAddress, helloReqMessage.ListenPort)
        };
}
