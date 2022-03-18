using Janus.Communication.NetworkAdapters;

namespace Janus.Communication.Nodes;

public sealed class MaskCommunicationNode : CommunicationNode
{
    internal MaskCommunicationNode(CommunicationNodeOptions options, IMaskNetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
    }

    public override NodeTypes NodeType => NodeTypes.MASK_NODE;
}
