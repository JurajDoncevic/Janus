using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Nodes;

public interface IWrapperCommunicationNode
    : ISendsCommandRes, ISendsQueryRes, ISendsSchemaRes,
      IReceivesCommandReq, IReceivesQueryReq, IReceivesSchemaReq
{
}
