using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.NetworkAdapters.Exceptions;
using Janus.Communication.Remotes;
using Janus.Utils.Logging;

namespace Janus.Communication.NetworkAdapters.Tcp;

public sealed class WrapperNetworkAdapter : NetworkAdapter, IWrapperNetworkAdapter
{
    private readonly ILogger<WrapperNetworkAdapter>? _logger;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="listenPort">TCP listening port</param>
    internal WrapperNetworkAdapter(int listenPort, ILogger<WrapperNetworkAdapter>? logger) : base(listenPort, logger)
    {
        _logger = logger;
    }

    public event EventHandler<CommandReqReceivedEventArgs>? CommandRequestReceived;
    public event EventHandler<QueryReqReceivedEventArgs>? QueryRequestReceived;
    public event EventHandler<SchemaReqReceivedEventArgs>? SchemaRequestReceived;

    public override DataResult<BaseMessage> BuildSpecializedMessage(string preambule, byte[] messageBytes)
        => preambule switch
        {
            Preambles.SCHEMA_REQUEST => MessageExtensions.ToSchemaReqMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.QUERY_REQUEST => MessageExtensions.ToQueryReqMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.COMMAND_REQUEST => MessageExtensions.ToCommandReqMessage(messageBytes).Map(_ => (BaseMessage)_),
            _ => ResultExtensions.AsDataResult<BaseMessage>(BaseMessage () => throw new UnknownMessageTypeException("Unknown message type"))
        };

    public override void RaiseSpecializedMessageReceivedEvent(BaseMessage message, string address)
    {
        switch (message.Preamble)
        {
            case Preambles.SCHEMA_REQUEST:
                SchemaRequestReceived?.Invoke(this, new SchemaReqReceivedEventArgs((SchemaReqMessage)message, address));
                break;
            case Preambles.QUERY_REQUEST:
                QueryRequestReceived?.Invoke(this, new QueryReqReceivedEventArgs((QueryReqMessage)message, address));
                break;
            case Preambles.COMMAND_REQUEST:
                CommandRequestReceived?.Invoke(this, new CommandReqReceivedEventArgs((CommandReqMessage)message, address));
                break;
            default:
                // do nothing
                break;
        }
    }

    public async Task<Result> SendCommandResponse(CommandResMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));

    public async Task<Result> SendQueryResponse(QueryResMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));

    public async Task<Result> SendSchemaResponse(SchemaResMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));
}
