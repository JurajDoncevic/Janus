namespace Janus.Communication.Nodes;

public interface IMaskCommunicationNode
    : ISendsCommandReq, ISendsQueryReq, ISendsSchemaReq,
      IReceivesCommandRes, IReceivesQueryRes, IReceivesSchemaRes,
      ICommunicationNode
{
}
