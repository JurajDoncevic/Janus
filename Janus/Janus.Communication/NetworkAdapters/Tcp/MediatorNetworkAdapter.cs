using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.NetworkAdapters.Exceptions;
using Janus.Communication.Remotes;
using Janus.Utils.Logging;

namespace Janus.Communication.NetworkAdapters.Tcp;

public sealed class MediatorNetworkAdapter : NetworkAdapter, IMediatorNetworkAdapter
{
    private readonly ILogger<MediatorNetworkAdapter>? _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="listenPort">TCP listening port</param>
    internal MediatorNetworkAdapter(int listenPort, ILogger<MediatorNetworkAdapter>? logger) : base(listenPort, logger)
    {
        _logger = logger;
    }

    public event EventHandler<QueryReqReceivedEventArgs>? QueryRequestReceived;
    public event EventHandler<QueryResReceivedEventArgs>? QueryResponseReceived;
    public event EventHandler<SchemaReqReceivedEventArgs>? SchemaRequestReceived;
    public event EventHandler<SchemaResReceivedEventArgs>? SchemaResponseReceived;
    public event EventHandler<CommandReqReceivedEventArgs>? CommandRequestReceived;
    public event EventHandler<CommandResReceivedEventArgs>? CommandResponseReceived;

    public override Result<BaseMessage> BuildSpecializedMessage(string preambule, byte[] messageBytes)
        => preambule switch
        {
            Preambles.SCHEMA_REQUEST => MessageExtensions.ToSchemaReqMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.QUERY_REQUEST => MessageExtensions.ToQueryReqMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.COMMAND_REQUEST => MessageExtensions.ToCommandReqMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.SCHEMA_RESPONSE => MessageExtensions.ToSchemaResMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.QUERY_RESPONSE => MessageExtensions.ToQueryResMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.COMMAND_RESPONSE => MessageExtensions.ToCommandResMessage(messageBytes).Map(_ => (BaseMessage)_),
            _ => ResultExtensions.AsResult<BaseMessage>(BaseMessage () => throw new UnknownMessageTypeException("Unknown message type"))
        };

    public override void RaiseSpecializedMessageReceivedEvent(BaseMessage message, string address)
    {
        switch (message.Preamble)
        {
            case Preambles.SCHEMA_REQUEST:
                SchemaRequestReceived?.Invoke(this, new SchemaReqReceivedEventArgs((SchemaReqMessage)message, address));
                _logger?.Debug("Invoked SchemaRequestReceived");
                break;
            case Preambles.QUERY_REQUEST:
                QueryRequestReceived?.Invoke(this, new QueryReqReceivedEventArgs((QueryReqMessage)message, address));
                _logger?.Debug("Invoked QueryRequestReceived");
                break;
            case Preambles.COMMAND_REQUEST:
                CommandRequestReceived?.Invoke(this, new CommandReqReceivedEventArgs((CommandReqMessage)message, address));
                _logger?.Debug("Invoked CommandRequestReceived");
                break;
            case Preambles.SCHEMA_RESPONSE:
                SchemaResponseReceived?.Invoke(this, new SchemaResReceivedEventArgs((SchemaResMessage)message, address));
                _logger?.Debug("Invoked SchemaResponseReceived");
                break;
            case Preambles.QUERY_RESPONSE:
                QueryResponseReceived?.Invoke(this, new QueryResReceivedEventArgs((QueryResMessage)message, address));
                _logger?.Debug("Invoked QueryResponseReceived");
                break;
            case Preambles.COMMAND_RESPONSE:
                CommandResponseReceived?.Invoke(this, new CommandResReceivedEventArgs((CommandResMessage)message, address));
                _logger?.Debug("Invoked CommandResponseReceived");
                break;
            default:
                // do nothing
                break;
        }
    }

    public async Task<Result> SendCommandRequest(CommandReqMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));

    public async Task<Result> SendCommandResponse(CommandResMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));

    public async Task<Result> SendQueryRequest(QueryReqMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));

    public async Task<Result> SendQueryResponse(QueryResMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));

    public async Task<Result> SendSchemaRequest(SchemaReqMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));

    public async Task<Result> SendSchemaResponse(SchemaResMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));
}
