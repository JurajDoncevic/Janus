using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.NetworkAdapters.Tcp;

public sealed class MaskNetworkAdapter : NetworkAdapter, IMaskNetworkAdapter
{
    internal MaskNetworkAdapter(int listenPort) : base(listenPort)
    {
    }

    public event EventHandler<CommandResReceivedEventArgs> CommandResponseReceived;
    public event EventHandler<QueryResReceivedEventArgs> QueryResponseReceived;
    public event EventHandler<SchemaResReceivedEventArgs> SchemaResponseReceived;

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

    public Result SendCommandRequest(CommandReqMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Result SendQueryRequest(QueryReqMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Result SendSchemaRequest(SchemaReqMessage message, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }
}
