using Janus.Commons.Nodes;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Implementations;

namespace Janus.Communication.Tests.Mocks;

public class AlwaysTimeoutCommunicationNode : BaseCommunicationNode<IMediatorNetworkAdapter>
{
    public AlwaysTimeoutCommunicationNode(CommunicationNodeOptions options!!, IMediatorNetworkAdapter networkAdapter) : base(options, networkAdapter)
    {

    }
    public override NodeTypes NodeType => NodeTypes.MEDIATOR;
}
