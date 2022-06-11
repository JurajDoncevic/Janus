using Janus.Communication.NetworkAdapters;

namespace Janus.Communication.Nodes.Implementations;

public sealed class MediatorCommunicationNode : BaseCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.MEDIATOR_NODE;

    internal MediatorCommunicationNode(CommunicationNodeOptions options, INetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
    }
}
