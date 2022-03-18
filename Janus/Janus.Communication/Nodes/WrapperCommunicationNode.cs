using Janus.Communication.NetworkAdapters;

namespace Janus.Communication.Nodes;

public sealed class WrapperCommunicationNode : CommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.WRAPPER_NODE;

    internal WrapperCommunicationNode(CommunicationNodeOptions options, INetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
    }
}
