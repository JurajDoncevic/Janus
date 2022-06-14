using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;
using Janus.Utils.Logging;

namespace Janus.Communication.Nodes.Implementations;

public sealed class WrapperCommunicationNode : BaseCommunicationNode<IWrapperNetworkAdapter>, IWrapperCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.WRAPPER_NODE;

    private readonly ILogger<WrapperCommunicationNode>? _logger;

    internal WrapperCommunicationNode(CommunicationNodeOptions options, IWrapperNetworkAdapter networkAdapter, ILogger<WrapperCommunicationNode>? logger = null) : base(options, networkAdapter, logger)
    {
        _logger = logger;

        networkAdapter.CommandRequestReceived += NetworkAdapter_CommandRequestReceived;
        networkAdapter.QueryRequestReceived += NetworkAdapter_QueryRequestReceived;
        networkAdapter.SchemaRequestReceived += NetworkAdapter_SchemaRequestReceived;
    }

    #region MANAGE INCOMING MESSAGES
    private void NetworkAdapter_SchemaRequestReceived(object? sender, NetworkAdapters.Events.SchemaReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info($"Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the requests
            var enqueued = _messageStore.EnqueueRequestInExchange(message.ExchangeId, message);
            if (enqueued)
            {
                _logger?.Info($"Added {0} from {1} in exchange {2} to received requests", message.Preamble, message.NodeId, message.ExchangeId);

                // raise event
                SchemaRequestReceived?.Invoke(this, new SchemaReqEventArgs(e.Message, remotePoint));
            }
        }
    }

    private void NetworkAdapter_QueryRequestReceived(object? sender, NetworkAdapters.Events.QueryReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info($"Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the requests
            var enqueued = _messageStore.EnqueueRequestInExchange(message.ExchangeId, message);
            if (enqueued)
            {
                _logger?.Info($"Added {0} from {1} in exchange {2} to received requests", message.Preamble, message.NodeId, message.ExchangeId);

                // raise event
                QueryRequestReceived?.Invoke(this, new QueryReqEventArgs(e.Message, remotePoint));
            }
        }
    }

    private void NetworkAdapter_CommandRequestReceived(object? sender, NetworkAdapters.Events.CommandReqReceivedEventArgs e)
    {
        // get the message
        var message = e.Message;
        _logger?.Info($"Managing {0} from node {1} in exchange {2}", message.Preamble, message.NodeId, message.ExchangeId);
        // is this a saved remote point and do the addresses match
        if (_remotePoints.ContainsKey(message.NodeId) && _remotePoints[message.NodeId].Address.Equals(e.SenderAddress))
        {
            var remotePoint = _remotePoints[message.NodeId];

            // add the message to the responses
            var enqueued = _messageStore.EnqueueResponseInExchange(message.ExchangeId, message);
            if (enqueued)
            {
                _logger?.Info($"Added {0} from {1} in exchange {2} to received requests", message.Preamble, message.NodeId, message.ExchangeId);

                // raise event
                CommandRequestReceived?.Invoke(this, new CommandReqEventArgs(e.Message, remotePoint));
            }
        }
    }
    #endregion

    #region RECEIVED MESSAGE EVENTS
    public event EventHandler<CommandReqEventArgs>? CommandRequestReceived;
    public event EventHandler<QueryReqEventArgs>? QueryRequestReceived;
    public event EventHandler<SchemaReqEventArgs>? SchemaRequestReceived;
    #endregion

    #region SEND MESSAGES
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
                result => _logger?.Info($"Sending {0} to {1} successful with exchange {2}", commandResponse.Preamble, remotePoint, commandResponse.ExchangeId),
                result => _logger?.Info($"Sending {0} to {1} failed with message {2}", commandResponse.Preamble, remotePoint, result.ErrorMessage)
            );
        return result;
    }

    public async Task<Result> SendQueryResponse(string exchangeId, TabularData queryResult, RemotePoint remotePoint, string? errorMessage = null, int blockNumber = 1, int totalBlocks = 1)
    {
        // create command response message
        var queryResponse = new QueryResMessage(exchangeId, _options.NodeId, queryResult, errorMessage, blockNumber, totalBlocks);

        // send command response with timeout
        var result =
            (await Timing.RunWithTimeout(
                async (token) => await _networkAdapter.SendQueryResponse(queryResponse, remotePoint).WaitAsync(token), // send the response
                _options.TimeoutMs))
            .Pass(
                result => _logger?.Info($"Sending {0} to {1} successful with exchange {2}", queryResponse.Preamble, remotePoint, queryResponse.ExchangeId),
                result => _logger?.Info($"Sending {0} to {1} failed with message {2}", queryResponse.Preamble, remotePoint, result.ErrorMessage)
            );
        return result;
    }

    public async Task<Result> SendSchemaResponse(string exchangeId, DataSource schema, RemotePoint remotePoint)
    {
        // create command response message
        var schemaResponse = new SchemaResMessage(exchangeId, _options.NodeId, schema);

        // send command response with timeout
        var result =
            (await Timing.RunWithTimeout(
                async (token) => await _networkAdapter.SendSchemaResponse(schemaResponse, remotePoint).WaitAsync(token), // send the response
                _options.TimeoutMs))
            .Pass(
                result => _logger?.Info($"Sending {0} to {1} successful with exchange {2}", schemaResponse.Preamble, remotePoint, schemaResponse.ExchangeId),
                result => _logger?.Info($"Sending {0} to {1} failed with message {2}", schemaResponse.Preamble, remotePoint, result.ErrorMessage)
            );
        return result;
    }
    #endregion
}
