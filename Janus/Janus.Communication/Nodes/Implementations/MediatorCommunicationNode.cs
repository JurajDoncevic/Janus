﻿using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Implementations;

public sealed class MediatorCommunicationNode : BaseCommunicationNode<IMediatorNetworkAdapter>, IMediatorCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.MEDIATOR_NODE;

    internal MediatorCommunicationNode(CommunicationNodeOptions options, IMediatorNetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
        networkAdapter.SchemaRequestReceived += NetworkAdapter_SchemaRequestReceived;
        networkAdapter.SchemaResponseReceived += NetworkAdapter_SchemaResponseReceived;
        networkAdapter.QueryRequestReceived += NetworkAdapter_QueryRequestReceived;
        networkAdapter.QueryResponseReceived += NetworkAdapter_QueryResponseReceived;
        networkAdapter.CommandRequestReceived += NetworkAdapter_CommandRequestReceived;
        networkAdapter.CommandResponseReceived += NetworkAdapter_CommandResponseReceived;
    }

    #region MANAGE INCOMING MESSAGES
    private void NetworkAdapter_CommandResponseReceived(object? sender, NetworkAdapters.Events.CommandResReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses dictionary
            _receivedResponseMessages.AddOrUpdate(message.ExchangeId, message, (k, v) => message);

            // raise event
            CommandResponseReceived?.Invoke(this, new CommandResEventArgs(e.Message, remotePoint));
        }
    }

    private void NetworkAdapter_CommandRequestReceived(object? sender, NetworkAdapters.Events.CommandReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses dictionary
            _receivedResponseMessages.AddOrUpdate(message.ExchangeId, message, (k, v) => message);

            // raise event
            CommandRequestReceived?.Invoke(this, new CommandReqEventArgs(e.Message, remotePoint));
        }
    }

    private void NetworkAdapter_QueryResponseReceived(object? sender, NetworkAdapters.Events.QueryResReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses dictionary
            _receivedResponseMessages.AddOrUpdate(message.ExchangeId, message, (k, v) => message);

            // raise event
            QueryResponseReceived?.Invoke(this, new QueryResEventArgs(e.Message, remotePoint));
        }
    }

    private void NetworkAdapter_QueryRequestReceived(object? sender, NetworkAdapters.Events.QueryReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses dictionary
            _receivedResponseMessages.AddOrUpdate(message.ExchangeId, message, (k, v) => message);

            // raise event
            QueryRequestReceived?.Invoke(this, new QueryReqEventArgs(e.Message, remotePoint));
        }
    }

    private void NetworkAdapter_SchemaResponseReceived(object? sender, NetworkAdapters.Events.SchemaResReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses dictionary
            _receivedResponseMessages.AddOrUpdate(message.ExchangeId, message, (k, v) => message);

            // raise event
            SchemaResponseReceived?.Invoke(this, new SchemaResEventArgs(e.Message, remotePoint));
        }
    }

    private void NetworkAdapter_SchemaRequestReceived(object? sender, NetworkAdapters.Events.SchemaReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses dictionary
            _receivedResponseMessages.AddOrUpdate(message.ExchangeId, message, (k, v) => message);

            // raise event
            SchemaRequestReceived?.Invoke(this, new SchemaReqEventArgs(e.Message, remotePoint));
        }
    }
    #endregion

    #region RECEIVED MESSAGE EVENTS
    public event EventHandler<QueryReqEventArgs>? QueryRequestReceived;
    public event EventHandler<QueryResEventArgs>? QueryResponseReceived;
    public event EventHandler<SchemaReqEventArgs>? SchemaRequestReceived;
    public event EventHandler<SchemaResEventArgs>? SchemaResponseReceived;
    public event EventHandler<CommandReqEventArgs>? CommandRequestReceived;
    public event EventHandler<CommandResEventArgs>? CommandResponseReceived;
    #endregion

    #region SEND MESSAGES
    public async Task<DataResult<TabularData>> SendQueryRequest(Query query, RemotePoint remotePoint)
    {
        // create query request message
        var queryRequest = new QueryReqMessage(_options.NodeId, query);
        var exchangdeId = queryRequest.ExchangeId;
        // send command request with timeout
        var result = Timing.RunWithTimeout(
            async (token) =>
                (await _networkAdapter.SendQueryRequest(queryRequest, remotePoint)) // send the request
                    .Bind(result =>
                    {
                        // wait for the response to appear
                        while (!_receivedResponseMessages.ContainsKey(exchangdeId))
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        // get the hello response - exception if not correct message type
                        var queryResponse = (QueryResMessage)_receivedResponseMessages[exchangdeId];
                        // remove the response from the concurrent dict
                        _receivedResponseMessages.Remove(exchangdeId, out _);
                        // have to throw exception to register as error and get outcome message
                        return ResultExtensions.AsDataResult(() => queryResponse.IsSuccess ? queryResponse.TabularData : throw new Exception(queryResponse.ErrorMessage));
                    }),
                _options.TimeoutMs);
        return await result;
    }

    public async Task<Result> SendQueryResponse(string exchangeId, TabularData queryResult, RemotePoint remotePoint, string? errorMessage = null, int blockNumber = 1, int totalBlocks = 1)
    {
        // create command response message
        var queryResponse = new QueryResMessage(exchangeId, _options.NodeId, queryResult, errorMessage, blockNumber, totalBlocks);

        // send command response with timeout
        var result = Timing.RunWithTimeout(
            async (token) =>
                await _networkAdapter.SendQueryResponse(queryResponse, remotePoint).WaitAsync(token), // send the response
                _options.TimeoutMs);
        return await result;
    }

    public async Task<DataResult<DataSource>> SendSchemaRequest(RemotePoint remotePoint)
    {
        // create query request message
        var schemaRequest = new SchemaReqMessage(_options.NodeId);
        var exchangdeId = schemaRequest.ExchangeId;
        // send command request with timeout
        var result = Timing.RunWithTimeout(
            async (token) =>
                (await _networkAdapter.SendSchemaRequest(schemaRequest, remotePoint)) // send the request
                    .Bind(result =>
                    {
                        // wait for the response to appear
                        while (!_receivedResponseMessages.ContainsKey(exchangdeId))
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        // get the hello response - exception if not correct message type
                        var schemaResponse = (SchemaResMessage)_receivedResponseMessages[exchangdeId];
                        // remove the response from the concurrent dict
                        _receivedResponseMessages.Remove(exchangdeId, out _);
                        // have to throw exception to register as error and get outcome message
                        return ResultExtensions.AsDataResult(() => schemaResponse.DataSource);
                    }),
                _options.TimeoutMs);
        return await result;
    }

    public async Task<Result> SendSchemaResponse(string exchangeId, DataSource schema, RemotePoint remotePoint)
    {
        // create command response message
        var schemaResponse = new SchemaResMessage(exchangeId, _options.NodeId, schema);

        // send command response with timeout
        var result = Timing.RunWithTimeout(
            async (token) =>
                await _networkAdapter.SendSchemaResponse(schemaResponse, remotePoint).WaitAsync(token), // send the response
                _options.TimeoutMs);
        return await result;
    }

    public async Task<Result> SendCommandRequest(BaseCommand command, RemotePoint remotePoint)
    {
        // create command request message
        var commandRequest = new CommandReqMessage(_options.NodeId, command);
        var exchangdeId = commandRequest.ExchangeId;
        // send command request with timeout
        var result = Timing.RunWithTimeout(
            async (token) =>
                (await _networkAdapter.SendCommandRequest(commandRequest, remotePoint)) // send the request
                    .Bind(result =>
                    {
                        // wait for the response to appear
                        while (!_receivedResponseMessages.ContainsKey(exchangdeId))
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        // get the hello response - exception if not correct message type
                        var commandResponse = (CommandResMessage)_receivedResponseMessages[exchangdeId];
                        // remove the response from the concurrent dict
                        _receivedResponseMessages.Remove(exchangdeId, out _);
                        // have to throw exception to register as error and get outcome message
                        return ResultExtensions.AsResult(() => commandResponse.IsSuccess ? true : throw new Exception(commandResponse.OutcomeDescription));
                    }),
                _options.TimeoutMs);
        return await result;
    }

    public async Task<Result> SendCommandResponse(string exchangeId, bool isSuccess, RemotePoint remotePoint, string outcomeDescription = "")
    {
        // create command response message
        var commandResponse = new CommandResMessage(exchangeId, _options.NodeId, isSuccess, outcomeDescription);

        // send command response with timeout
        var result = Timing.RunWithTimeout(
            async (token) =>
                await _networkAdapter.SendCommandResponse(commandResponse, remotePoint).WaitAsync(token), // send the response
                _options.TimeoutMs);
        return await result;
    }
    #endregion
}
