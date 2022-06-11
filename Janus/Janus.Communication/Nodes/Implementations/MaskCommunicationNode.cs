using Janus.Commons.CommandModels;
using Janus.Commons.QueryModels;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Implementations;

public sealed class MaskCommunicationNode : BaseCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.MASK_NODE;

    internal MaskCommunicationNode(CommunicationNodeOptions options, IMaskNetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
    }
}
