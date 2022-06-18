namespace Janus.Communication.NetworkAdapters;

public interface IMediatorNetworkAdapter
    : INetworkAdapter,
      ISendsQueryReq, ISendsQueryRes, ISendsSchemaReq, ISendsSchemaRes, ISendsCommandReq, ISendsCommandRes,
      IReceivesQueryReq, IReceivesQueryRes, IReceivesSchemaReq, IReceivesSchemaRes, IReceivesCommandReq, IReceivesCommandRes
{
}
