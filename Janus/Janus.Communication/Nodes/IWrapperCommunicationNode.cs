namespace Janus.Communication.Nodes;

public interface IWrapperCommunicationNode
    : ISendsCommandRes, ISendsQueryRes, ISendsSchemaRes,
      IReceivesCommandReq, IReceivesQueryReq, IReceivesSchemaReq
{
}
