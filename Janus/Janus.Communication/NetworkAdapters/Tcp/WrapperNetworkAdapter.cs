using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.NetworkAdapters.Exceptions;
using Janus.Communication.Remotes;
using System.Net.Sockets;

namespace Janus.Communication.NetworkAdapters.Tcp;

public sealed class WrapperNetworkAdapter : NetworkAdapter, IWrapperNetworkAdapter
{
    internal WrapperNetworkAdapter(int listenPort) : base(listenPort)
    {
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
        => await SendMessageBytes(message.ToBson(), remotePoint);

    public async Task<Result> SendQueryResponse(QueryResMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint);

    public async Task<Result> SendSchemaResponse(SchemaResMessage message, RemotePoint remotePoint)
        => await SendMessageBytes(message.ToBson(), remotePoint);
}
