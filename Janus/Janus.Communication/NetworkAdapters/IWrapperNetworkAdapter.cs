namespace Janus.Communication.NetworkAdapters;

public interface IWrapperNetworkAdapter
    : INetworkAdapter,
      ISendsCommandRes, ISendsQueryRes, ISendsSchemaRes,
      IReceivesCommandReq, IReceivesQueryReq, IReceivesSchemaReq
{
}
