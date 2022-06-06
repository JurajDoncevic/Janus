using Janus.Communication.Messages;
using Janus.Communication.NetworkAdapters.Events;
using Janus.Communication.Remotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.NetworkAdapters;

public interface IMediatorNetworkAdapter 
    : INetworkAdapter,
    ISendsQueryReq, ISendsQueryRes, ISendsSchemaReq, ISendsSchemaRes, ISendsCommandReq, ISendsCommandRes,
    IReceivesQueryReq, IReceivesQueryRes, IReceivesSchemaReq, IReceivesSchemaRes, IReceivesCommandReq, IReceivesCommandRes
{
}
