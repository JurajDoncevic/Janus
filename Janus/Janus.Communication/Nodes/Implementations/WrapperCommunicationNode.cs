using Janus.Communication.NetworkAdapters;

namespace Janus.Communication.Nodes.Implementations;

public sealed class WrapperCommunicationNode : BaseCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.WRAPPER_NODE;

    internal WrapperCommunicationNode(CommunicationNodeOptions options, INetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
    }
}
