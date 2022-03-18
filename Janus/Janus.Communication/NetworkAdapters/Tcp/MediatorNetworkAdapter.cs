using Janus.Communication.Messages;

namespace Janus.Communication.NetworkAdapters.Tcp;

public sealed class MediatorNetworkAdapter : NetworkAdapter, IMediatorNetworkAdapter
{
    internal MediatorNetworkAdapter(int listenPort) : base(listenPort)
    {
    }

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
}
