namespace Janus.Communication.Nodes;

public interface IMediatorCommunicationNode
    : ISendsQueryReq, ISendsQueryRes, ISendsSchemaReq, ISendsSchemaRes, ISendsCommandReq, ISendsCommandRes,
      IReceivesQueryReq, IReceivesQueryRes, IReceivesSchemaReq, IReceivesSchemaRes, IReceivesCommandReq, IReceivesCommandRes,
      ICommunicationNode
{
}
