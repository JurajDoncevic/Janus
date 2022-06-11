using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Nodes;

public interface IMediatorCommunicationNode
    : ISendsQueryReq, ISendsQueryRes, ISendsSchemaReq, ISendsSchemaRes, ISendsCommandReq, ISendsCommandRes,
      IReceivesQueryReq, IReceivesQueryRes, IReceivesSchemaReq, IReceivesSchemaRes, IReceivesCommandReq, IReceivesCommandRes
{
}
