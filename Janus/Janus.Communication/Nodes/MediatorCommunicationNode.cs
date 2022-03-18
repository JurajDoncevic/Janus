using Janus.Communication.NetworkAdapters;

namespace Janus.Communication.Nodes;

public sealed class MediatorCommunicationNode : CommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.MEDIATOR_NODE;

    internal MediatorCommunicationNode(CommunicationNodeOptions options, INetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
    }
}
