using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.NetworkAdapters.Exceptions;
using Janus.Communication.Remotes;
using Janus.Utils.Logging;
using System.Net.Sockets;

namespace Janus.Communication.NetworkAdapters.Tcp;

public sealed class MaskNetworkAdapter : NetworkAdapter, IMaskNetworkAdapter
{
    private readonly ILogger<MaskNetworkAdapter>? _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="listenPort">TCP listening port</param>
    internal MaskNetworkAdapter(int listenPort, ILogger<MaskNetworkAdapter>? logger = null) : base(listenPort, logger)
    {
        _logger = logger;
    }

    public event EventHandler<CommandResReceivedEventArgs>? CommandResponseReceived;
    public event EventHandler<QueryResReceivedEventArgs>? QueryResponseReceived;
    public event EventHandler<SchemaResReceivedEventArgs>? SchemaResponseReceived;

    public override DataResult<BaseMessage> BuildSpecializedMessage(string preambule, byte[] messageBytes)
        => preambule switch
        {
            Preambles.SCHEMA_RESPONSE => MessageExtensions.ToSchemaResMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.QUERY_RESPONSE => MessageExtensions.ToQueryResMessage(messageBytes).Map(_ => (BaseMessage)_),
            Preambles.COMMAND_RESPONSE => MessageExtensions.ToCommandResMessage(messageBytes).Map(_ => (BaseMessage)_),
            _ => ResultExtensions.AsDataResult<BaseMessage>(BaseMessage () => throw new UnknownMessageTypeException("Unknown message type")) 
        };

    public override void RaiseSpecializedMessageReceivedEvent(BaseMessage message, string address)
    {
        switch (message.Preamble)
        {
            case Preambles.SCHEMA_RESPONSE:
                SchemaResponseReceived?.Invoke(this, new SchemaResReceivedEventArgs((SchemaResMessage)message, address));
                break;
            case Preambles.QUERY_RESPONSE:
                QueryResponseReceived?.Invoke(this, new QueryResReceivedEventArgs((QueryResMessage)message, address));
                break;
            case Preambles.COMMAND_RESPONSE:
                CommandResponseReceived?.Invoke(this, new CommandResReceivedEventArgs((CommandResMessage)message, address));
                break;
            default:
                // do nothing
                break;
        }
    }

    public async Task<Result> SendCommandRequest(CommandReqMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));

    public async Task<Result> SendQueryRequest(QueryReqMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));

    public async Task<Result> SendSchemaRequest(SchemaReqMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint)
            .Pass(r => _logger?.Info($"Sending {0} to {1}", message.Preamble, remotePoint));
}
