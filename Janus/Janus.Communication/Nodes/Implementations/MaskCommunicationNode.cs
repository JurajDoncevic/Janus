using Janus.Commons.CommandModels;
using Janus.Commons.QueryModels;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Implementations;

public sealed class MaskCommunicationNode : BaseCommunicationNode<IMaskNetworkAdapter>, IMaskCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.MASK_NODE;

    internal MaskCommunicationNode(CommunicationNodeOptions options, IMaskNetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
        networkAdapter.SchemaResponseReceived += NetworkAdapter_SchemaResponseReceived;
        networkAdapter.QueryResponseReceived += NetworkAdapter_QueryResponseReceived;
        networkAdapter.CommandResponseReceived += NetworkAdapter_CommandResponseReceived;
    }

    #region MANAGE INCOMING MESSAGES
    private void NetworkAdapter_CommandResponseReceived(object? sender, NetworkAdapters.Events.CommandResReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NetworkAdapter_QueryResponseReceived(object? sender, NetworkAdapters.Events.QueryResReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NetworkAdapter_SchemaResponseReceived(object? sender, NetworkAdapters.Events.SchemaResReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region RECEIVED MESSAGE EVENTS
    public event EventHandler<CommandResEventArgs>? CommandResponseReceived;
    public event EventHandler<QueryResEventArgs>? QueryResponseReceived;
    public event EventHandler<SchemaResEventArgs>? SchemaResponseReceived;
    #endregion

    #region SEND MESSAGES
    public Task<Result> SendCommandRequest(BaseCommand command, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendQueryRequest(Query query, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendSchemaRequest(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }
    #endregion
}
