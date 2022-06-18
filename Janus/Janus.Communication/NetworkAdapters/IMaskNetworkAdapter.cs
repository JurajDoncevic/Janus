namespace Janus.Communication.NetworkAdapters;

public interface IMaskNetworkAdapter
    : INetworkAdapter,
      ISendsCommandReq, ISendsQueryReq, ISendsSchemaReq,
      IReceivesCommandRes, IReceivesQueryRes, IReceivesSchemaRes
{
}
