﻿using Janus.Commons.Nodes;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;

namespace Janus.Communication.Tests.Mocks;

public class AlwaysTimeoutCommunicationNode : BaseCommunicationNode<IMediatorNetworkAdapter>
{
    public AlwaysTimeoutCommunicationNode(CommunicationNodeOptions options, IMediatorNetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }
    }
    public override NodeTypes NodeType => NodeTypes.MEDIATOR;

    protected override Result IsRemotePointOkToRegister(RemotePoint remotePoint)
    {
        return Results.OnSuccess();
    }
}
