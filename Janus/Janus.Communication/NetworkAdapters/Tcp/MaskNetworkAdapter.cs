using Janus.Commons.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.NetworkAdapters.Exceptions;
using Janus.Communication.Remotes;
using Janus.Serialization;
using Janus.Logging;

namespace Janus.Communication.NetworkAdapters.Tcp;

public sealed class MaskNetworkAdapter : NetworkAdapter, IMaskNetworkAdapter
{
    private readonly ILogger<MaskNetworkAdapter>? _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="listenPort">TCP listening port</param>
    internal MaskNetworkAdapter(int listenPort, IBytesSerializationProvider serializationProvider, ILogger? logger = null) : base(listenPort, serializationProvider, logger)
    {
        _logger = logger?.ResolveLogger<MaskNetworkAdapter>();
    }

    public event EventHandler<CommandResReceivedEventArgs>? CommandResponseReceived;
    public event EventHandler<QueryResReceivedEventArgs>? QueryResponseReceived;
    public event EventHandler<SchemaResReceivedEventArgs>? SchemaResponseReceived;

    public override Result<BaseMessage> BuildSpecializedMessage(string preambule, byte[] messageBytes)
        => preambule switch
        {
            Preambles.SCHEMA_RESPONSE => _serializationProvider.SchemaResMessageSerializer.Deserialize(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.QUERY_RESPONSE => _serializationProvider.QueryResMessageSerializer.Deserialize(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.COMMAND_RESPONSE => _serializationProvider.CommandResMessageSerializer.Deserialize(messageBytes).Map(_ => (BaseMessage)_),
            _ => ResultExtensions.AsResult<BaseMessage>(BaseMessage () => throw new UnknownMessageTypeException("Unknown message type"))
        };

    public override void RaiseSpecializedMessageReceivedEvent(BaseMessage message, string address)
    {
        switch (message.Preamble)
        {
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
        => await Task.FromResult(_serializationProvider.CommandReqMessageSerializer.Serialize(message))
            .Bind(async messageBytes => await SendMessageBytes(messageBytes, remotePoint)
                                            .Pass(result => _logger?.Info("Sending {0} to {1}", message.Preamble, remotePoint)));

    public async Task<Result> SendQueryRequest(QueryReqMessage message, RemotePoint remotePoint)
        => await Task.FromResult(_serializationProvider.QueryReqMessageSerializer.Serialize(message))
            .Bind(async messageBytes => await SendMessageBytes(messageBytes, remotePoint)
                                            .Pass(result => _logger?.Info("Sending {0} to {1}", message.Preamble, remotePoint)));

    public async Task<Result> SendSchemaRequest(SchemaReqMessage message, RemotePoint remotePoint)
        => await Task.FromResult(_serializationProvider.SchemaReqMessageSerializer.Serialize(message))
            .Bind(async messageBytes => await SendMessageBytes(messageBytes, remotePoint)
                                            .Pass(result => _logger?.Info("Sending {0} to {1}", message.Preamble, remotePoint)));
}
