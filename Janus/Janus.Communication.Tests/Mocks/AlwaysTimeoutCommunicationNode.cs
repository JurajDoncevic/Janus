using Janus.Commons.Nodes;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;

namespace Janus.Communication.Tests.Mocks;

public class AlwaysTimeoutCommunicationNode : BaseCommunicationNode<IMediatorNetworkAdapter>
{
    public AlwaysTimeoutCommunicationNode(CommunicationNodeOptions options!!, IMediatorNetworkAdapter networkAdapter) : base(options, networkAdapter)
    {

    }
    public override NodeTypes NodeType => NodeTypes.MEDIATOR;

    protected override Result IsRemotePointOkToRegister(RemotePoint remotePoint)
    {
        return Result.OnSuccess();
    }
}
