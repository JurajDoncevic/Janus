﻿using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.Messages;
using Janus.Commons.Nodes;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;
using Janus.Logging;

namespace Janus.Communication.Nodes.Implementations;

public sealed class MediatorCommunicationNode : BaseCommunicationNode<IMediatorNetworkAdapter>, IMediatorCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.MEDIATOR;

    private readonly ILogger<MediatorCommunicationNode>? _logger;

    internal MediatorCommunicationNode(CommunicationNodeOptions options, IMediatorNetworkAdapter networkAdapter, ILogger? logger = null) : base(options, networkAdapter, logger)
    {
        _logger = logger?.ResolveLogger<MediatorCommunicationNode>();

        networkAdapter.SchemaRequestReceived += NetworkAdapter_SchemaRequestReceived;
        networkAdapter.SchemaResponseReceived += NetworkAdapter_SchemaResponseReceived;
        networkAdapter.QueryRequestReceived += NetworkAdapter_QueryRequestReceived;
        networkAdapter.QueryResponseReceived += NetworkAdapter_QueryResponseReceived;
        networkAdapter.CommandRequestReceived += NetworkAdapter_CommandRequestReceived;
        networkAdapter.CommandResponseReceived += NetworkAdapter_CommandResponseReceived;
    }

    #region MANAGE INCOMING MESSAGES
    private void NetworkAdapter_SchemaRequestReceived(object? sender, NetworkAdapters.Events.SchemaReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info("Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the requests
            var enqueued = _messageStore.EnqueueRequestInExchange(message.ExchangeId, message);
            if (enqueued)
            {
                _logger?.Info("Added {0} from {1} in exchange {2} to received requests", message.Preamble, message.NodeId, message.ExchangeId);

                // raise event
                SchemaRequestReceived?.Invoke(this, new SchemaReqEventArgs(e.Message, remotePoint));
            }
        }
    }

    private void NetworkAdapter_QueryRequestReceived(object? sender, NetworkAdapters.Events.QueryReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info("Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the requests
            var enqueued = _messageStore.EnqueueRequestInExchange(message.ExchangeId, message);
            if (enqueued)
            {
                _logger?.Info("Added {0} from {1} in exchange {2} to received requests", message.Preamble, message.NodeId, message.ExchangeId);

                // raise event
                QueryRequestReceived?.Invoke(this, new QueryReqEventArgs(e.Message, remotePoint));
            }
        }
    }

    private void NetworkAdapter_CommandRequestReceived(object? sender, NetworkAdapters.Events.CommandReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info("Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the requests
            var enqueued = _messageStore.EnqueueRequestInExchange(message.ExchangeId, message);
            if (enqueued)
            {
                _logger?.Info("Added {0} from {1} in exchange {2} to received requests", message.Preamble, message.NodeId, message.ExchangeId);

                // raise event
                CommandRequestReceived?.Invoke(this, new CommandReqEventArgs(e.Message, remotePoint));
            }
        }
    }

    private void NetworkAdapter_CommandResponseReceived(object? sender, NetworkAdapters.Events.CommandResReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info("Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses
            var enqueued = _messageStore.EnqueueResponseInExchange(message.ExchangeId, message);
            if (enqueued)
            {
                _logger?.Info("Added {0} from {1} in exchange {2} to received responses", message.Preamble, message.NodeId, message.ExchangeId);

                // raise event
                CommandResponseReceived?.Invoke(this, new CommandResEventArgs(e.Message, remotePoint));
            }
            else
            {
                _logger?.Info("Unknown exchange {0} for {1}", message.ExchangeId, message.Preamble);
            }
        }
    }

    private void NetworkAdapter_QueryResponseReceived(object? sender, NetworkAdapters.Events.QueryResReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info("Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses dictionary
            var enqueued = _messageStore.EnqueueResponseInExchange(message.ExchangeId, message);
            if (enqueued)
            {
                _logger?.Info("Added {0} from {1} in exchange {2} to received responses", message.Preamble, message.NodeId, message.ExchangeId);

                // raise event
                QueryResponseReceived?.Invoke(this, new QueryResEventArgs(e.Message, remotePoint));
            }
            else
            {
                _logger?.Info("Unknown exchange {0} for {1}", message.ExchangeId, message.Preamble);
            }
        }
    }

    private void NetworkAdapter_SchemaResponseReceived(object? sender, NetworkAdapters.Events.SchemaResReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info("Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses
            var enqueued = _messageStore.EnqueueResponseInExchange(message.ExchangeId, message);
            if (enqueued)
            {
                _logger?.Info("Added {0} from {1} in exchange {2} to received responses", message.Preamble, message.NodeId, message.ExchangeId);

                // raise event
                SchemaResponseReceived?.Invoke(this, new SchemaResEventArgs(e.Message, remotePoint));
            }
            else
            {
                _logger?.Info("Unknown exchange {0} for {1}", message.ExchangeId, message.Preamble);
            }
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
    public async Task<Result> SendCommandRequest(BaseCommand command, RemotePoint remotePoint)
    {
        // create command request message
        var commandRequest = new CommandReqMessage(_options.NodeId, command);
        var exchangeId = commandRequest.ExchangeId;

        // register the exchange so a response can be received
        _messageStore.RegisterExchange(exchangeId);

        // send command request with timeout
        var result = await Timing.RunWithTimeout(
            async (token) =>
                (await _networkAdapter.SendCommandRequest(commandRequest, remotePoint)) // send the request
                    .Pass(
                        result => _logger?.Info("Sending {0} to {1} successful with exchange {2}", commandRequest.Preamble, remotePoint, commandRequest.ExchangeId),
                        result => _logger?.Info("Sending {0} to {1} failed with message {2}", commandRequest.Preamble, remotePoint, result.Message)
                    )
                    .Bind(result => Results.AsResult(() =>
                    {
                        // wait for the response to appear
                        while (!_messageStore.AnyResponsesExist(exchangeId))
                        {
                            if (token.IsCancellationRequested)
                            {
                                _logger?.Info("Timeout reached waiting for {0} response in exchange {1}", commandRequest.Preamble, commandRequest.ExchangeId);
                                token.ThrowIfCancellationRequested();
                            }

                        }
                        // get the hello response - exception if not correct message type
                        var commandResponse = (CommandResMessage)_messageStore.DequeueResponseFromExchange(exchangeId).Data;
                        _logger?.Info("Received {0} on exchange {1} from {2}", commandResponse.Preamble, commandResponse.ExchangeId, commandResponse.NodeId);
                        // return outcome as Result; pack the outcome although it might be empty
                        return commandResponse.IsSuccess
                            ? Results.OnSuccess($"Command response indicates success: {commandResponse.OutcomeDescription}")
                            : Results.OnFailure($"Command response indicates failure: {commandResponse.OutcomeDescription}");
                    })),
                _options.TimeoutMs);

        // unregister the exchange
        _messageStore.UnregisterExchange(exchangeId);

        return result;
    }

    public async Task<Result<TabularData>> SendQueryRequest(Query query, RemotePoint remotePoint)
    {
        // create query request message
        var queryRequest = new QueryReqMessage(_options.NodeId, query);
        var exchangeId = queryRequest.ExchangeId;

        // register the exchange so a response can be received
        _messageStore.RegisterExchange(exchangeId);

        // send command request with timeout
        var result = await Timing.RunWithTimeout(
            async (token) =>
                (await _networkAdapter.SendQueryRequest(queryRequest, remotePoint)) // send the request
                    .Pass(
                        result => _logger?.Info("Sending {0} to {1} successful with exchange {2}", queryRequest.Preamble, remotePoint, queryRequest.ExchangeId),
                        result => _logger?.Info("Sending {0} to {1} failed with message {2}", queryRequest.Preamble, remotePoint, result.Message)
                    )
                    .Bind(result => Results.AsResult(() =>
                    {
                        // wait for the response to appear
                        while (!_messageStore.AnyResponsesExist(exchangeId))
                        {
                            if (token.IsCancellationRequested)
                            {
                                _logger?.Info("Timeout reached waiting for {0} response in exchange {1}", queryRequest.Preamble, queryRequest.ExchangeId);
                                token.ThrowIfCancellationRequested();
                            }

                        }
                        // get the hello response - exception if not correct message type
                        var queryResponse = (QueryResMessage)_messageStore.DequeueResponseFromExchange(exchangeId).Data;
                        _logger?.Info("Received returned {0} from {1} in exchange {2}", queryResponse.Preamble, queryResponse.NodeId, queryResponse.ExchangeId);

                        // determine query operation outcome from message and return appropriate Result
                        return queryResponse.IsSuccess
                            ? Results.OnSuccess(queryResponse.TabularData.Value) // if response is succcess, then there is always tabular data
                            : Results.OnFailure<TabularData>(queryResponse.OutcomeDescription);
                    })),
                _options.TimeoutMs);

        // unregister the exchange
        _messageStore.UnregisterExchange(exchangeId);

        return result;
    }

    public async Task<Result<DataSource>> SendSchemaRequest(RemotePoint remotePoint)
    {
        // create query request message
        var schemaRequest = new SchemaReqMessage(_options.NodeId);
        var exchangeId = schemaRequest.ExchangeId;

        // register the exchange so a response can be received
        _messageStore.RegisterExchange(exchangeId);

        // send command request with timeout
        var result = await Timing.RunWithTimeout(
            async (token) =>
                (await _networkAdapter.SendSchemaRequest(schemaRequest, remotePoint)) // send the request
                    .Pass(
                        result => _logger?.Info("Sending {0} to {1} successful with exchange {2}", schemaRequest.Preamble, remotePoint, schemaRequest.ExchangeId),
                        result => _logger?.Info("Sending {0} to {1} failed with message {2}", schemaRequest.Preamble, remotePoint, result.Message)
                    )
                    .Bind(result => Results.AsResult(() =>
                    {
                        // wait for the response to appear
                        while (!_messageStore.AnyResponsesExist(exchangeId))
                        {
                            if (token.IsCancellationRequested)
                            {
                                _logger?.Info("Timeout reached waiting for {0} response in exchange {1}", schemaRequest.Preamble, schemaRequest.ExchangeId);
                                token.ThrowIfCancellationRequested();
                            }
                        }
                        // get the hello response - exception if not correct message type
                        var schemaResponse = (SchemaResMessage)_messageStore.DequeueResponseFromExchange(exchangeId).Data;
                        _logger?.Info("Received returned {0} from {1} in exchange {2}", schemaResponse.Preamble, schemaResponse.NodeId, schemaResponse.ExchangeId);

                        // determine request operation outcome and return Result
                        return schemaResponse.IsSuccess
                            ? Results.OnSuccess(schemaResponse.DataSource.Value) // if response is successful, data source is always Some
                            : Results.OnFailure<DataSource>(schemaResponse.OutcomeDescription);
                    })),
                _options.TimeoutMs);

        // unregister the exchange
        _messageStore.UnregisterExchange(exchangeId);

        return result;
    }

    public async Task<Result> SendCommandResponse(string exchangeId, bool isSuccess, RemotePoint remotePoint, string outcomeDescription = "")
    {
        // create command response message
        var commandResponse = new CommandResMessage(exchangeId, _options.NodeId, isSuccess, outcomeDescription);

        // send command response with timeout
        var result =
            (await Timing.RunWithTimeout(
                async (token) => await _networkAdapter.SendCommandResponse(commandResponse, remotePoint).WaitAsync(token), // send the response
                _options.TimeoutMs))
            .Pass(
                result => _logger?.Info("Sending {0} to {1} successful with exchange {2}", commandResponse.Preamble, remotePoint, commandResponse.ExchangeId),
                result => _logger?.Info("Sending {0} to {1} failed with message {2}", commandResponse.Preamble, remotePoint, result.Message)
            );
        return result;
    }

    public async Task<Result> SendQueryResponse(string exchangeId, TabularData? queryResult, RemotePoint remotePoint, string? errorMessage = null, int blockNumber = 1, int totalBlocks = 1)
    {
        // create command response message
        var queryResponse = new QueryResMessage(exchangeId, _options.NodeId, queryResult, errorMessage, blockNumber, totalBlocks);

        // send command response with timeout
        var result =
            (await Timing.RunWithTimeout(
                async (token) => await _networkAdapter.SendQueryResponse(queryResponse, remotePoint).WaitAsync(token), // send the response
                _options.TimeoutMs))
            .Pass(
                result => _logger?.Info("Sending {0} to {1} successful with exchange {2}", queryResponse.Preamble, remotePoint, queryResponse.ExchangeId),
                result => _logger?.Info("Sending {0} to {1} failed with message {2}", queryResponse.Preamble, remotePoint, result.Message)
            );
        return result;
    }

    public async Task<Result> SendSchemaResponse(string exchangeId, DataSource? schema, RemotePoint remotePoint, string? outcomeDescription = null)
    {
        // create command response message
        var schemaResponse = new SchemaResMessage(exchangeId, _options.NodeId, schema, outcomeDescription);

        // send command response with timeout
        var result =
            (await Timing.RunWithTimeout(
                async (token) => await _networkAdapter.SendSchemaResponse(schemaResponse, remotePoint).WaitAsync(token), // send the response
                _options.TimeoutMs))
            .Pass(
                result => _logger?.Info("Sending {0} to {1} successful with exchange {2}", schemaResponse.Preamble, remotePoint, schemaResponse.ExchangeId),
                result => _logger?.Info("Sending {0} to {1} failed with message {2}", schemaResponse.Preamble, remotePoint, result.Message)
            );
        return result;
    }
    #endregion

    protected override Result IsRemotePointOkToRegister(RemotePoint remotePoint)
    {
        if (Options.NodeId.Equals(remotePoint.NodeId))
            return Results.OnFailure($"This node has the same id {Options.NodeId}.");

        return Results.OnSuccess();
    }
}
