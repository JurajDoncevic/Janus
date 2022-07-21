using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.Messages;
using Janus.Commons.Nodes;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;
using Janus.Utils.Logging;

namespace Janus.Communication.Nodes.Implementations;

public sealed class MaskCommunicationNode : BaseCommunicationNode<IMaskNetworkAdapter>, IMaskCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.MASK_NODE;

    private readonly ILogger<MaskCommunicationNode>? _logger;

    internal MaskCommunicationNode(CommunicationNodeOptions options, IMaskNetworkAdapter networkAdapter, ILogger? logger = null) : base(options, networkAdapter, logger)
    {
        _logger = logger?.ResolveLogger<MaskCommunicationNode>();

        networkAdapter.SchemaResponseReceived += NetworkAdapter_SchemaResponseReceived;
        networkAdapter.QueryResponseReceived += NetworkAdapter_QueryResponseReceived;
        networkAdapter.CommandResponseReceived += NetworkAdapter_CommandResponseReceived;
    }

    #region MANAGE INCOMING MESSAGES
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
    public event EventHandler<CommandResEventArgs>? CommandResponseReceived;
    public event EventHandler<QueryResEventArgs>? QueryResponseReceived;
    public event EventHandler<SchemaResEventArgs>? SchemaResponseReceived;
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
                    .Bind(result => ResultExtensions.AsResult(() =>
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
                        // have to throw exception to register as error and get outcome message
                        return commandResponse.IsSuccess ? true : throw new Exception(commandResponse.OutcomeDescription);
                    })),
                _options.TimeoutMs);

        // unregister the exchange
        _messageStore.RegisterExchange(exchangeId);

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
                    .Bind(result => ResultExtensions.AsResult(() =>
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

                        // have to throw exception to register as error and get outcome message
                        return queryResponse.IsSuccess ? queryResponse.TabularData : throw new Exception(queryResponse.ErrorMessage);
                    })),
                _options.TimeoutMs);

        // unregister the exchange
        _messageStore.RegisterExchange(exchangeId);

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
                    .Bind(result => ResultExtensions.AsResult(() =>
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

                        // have to throw exception to register as error and get outcome message
                        return schemaResponse.DataSource;
                    })),
                _options.TimeoutMs);

        // unregister the exchange
        _messageStore.RegisterExchange(exchangeId);

        return result;
    }
    #endregion
}
