using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Remotes;

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
    {
        return ResultExtensions.AsDataResult(() =>
        {
            throw new NotImplementedException();
            return (BaseMessage)null;
        });
    }

    public override void RaiseSpecializedMessageReceivedEvent(BaseMessage message, string address)
    {

    }

    public Result SendCommandResponse(CommandResMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Result SendQueryResponse(QueryResMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Result SendSchemaResponse(SchemaResMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }
}
