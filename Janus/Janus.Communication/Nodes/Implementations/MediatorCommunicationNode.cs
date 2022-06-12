using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Implementations;

public sealed class MediatorCommunicationNode : BaseCommunicationNode<IMediatorNetworkAdapter>, IMediatorCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.MEDIATOR_NODE;

    internal MediatorCommunicationNode(CommunicationNodeOptions options, IMediatorNetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
        networkAdapter.SchemaRequestReceived += NetworkAdapter_SchemaRequestReceived;
        networkAdapter.SchemaResponseReceived += NetworkAdapter_SchemaResponseReceived;
        networkAdapter.QueryRequestReceived += NetworkAdapter_QueryRequestReceived;
        networkAdapter.QueryResponseReceived += NetworkAdapter_QueryResponseReceived;
        networkAdapter.CommandRequestReceived += NetworkAdapter_CommandRequestReceived;
        networkAdapter.CommandResponseReceived += NetworkAdapter_CommandResponseReceived;
    }

    #region MANAGE INCOMING MESSAGES
    private void NetworkAdapter_CommandResponseReceived(object? sender, NetworkAdapters.Events.CommandResReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NetworkAdapter_CommandRequestReceived(object? sender, NetworkAdapters.Events.CommandReqReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NetworkAdapter_QueryResponseReceived(object? sender, NetworkAdapters.Events.QueryResReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NetworkAdapter_QueryRequestReceived(object? sender, NetworkAdapters.Events.QueryReqReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NetworkAdapter_SchemaResponseReceived(object? sender, NetworkAdapters.Events.SchemaResReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NetworkAdapter_SchemaRequestReceived(object? sender, NetworkAdapters.Events.SchemaReqReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region RECEIVED MESSAGE EVENTS
    public event EventHandler<QueryReqEventArgs>? QueryRequestReceived;
    public event EventHandler<QueryResEventArgs>? QueryResponseReceived;
    public event EventHandler<SchemaReqEventArgs>? SchemaRequestReceived;
    public event EventHandler<SchemaResEventArgs>? SchemaResponseReceived;
    public event EventHandler<CommandReqEventArgs>? CommandRequestReceived;
    public event EventHandler<CommandResEventArgs>? CommandResponseReceived;
    #endregion

    #region SEND MESSAGES
    public Task<DataResult<TabularData>> SendQueryRequest(Query query, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendQueryResponse(TabularData queryResult, RemotePoint remotePoint, string? errorMessage = null, int blockNumber = 1, int totalBlocks = 1)
    {
        throw new NotImplementedException();
    }

    public Task<DataResult<DataSource>> SendSchemaRequest(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendSchemaResponse(DataSource schema, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendCommandRequest(BaseCommand command, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendCommandResponse(bool isSuccess, RemotePoint remotePoint, string outcomeDescription = "")
    {
        throw new NotImplementedException();
    }
    #endregion
}
