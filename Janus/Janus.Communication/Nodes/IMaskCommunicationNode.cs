using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Nodes;

public interface IMaskCommunicationNode
    : ISendsCommandReq, ISendsQueryReq, ISendsSchemaReq,
      IReceivesCommandRes, IReceivesQueryRes, IReceivesSchemaRes
{
}
