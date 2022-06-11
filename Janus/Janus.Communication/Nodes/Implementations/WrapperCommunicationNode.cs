using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Events;
using Janus.Communication.Remotes;

namespace Janus.Communication.Nodes.Implementations;

public sealed class WrapperCommunicationNode : BaseCommunicationNode<IWrapperNetworkAdapter>, IWrapperCommunicationNode
{
    public override NodeTypes NodeType => NodeTypes.WRAPPER_NODE;

    internal WrapperCommunicationNode(CommunicationNodeOptions options, IWrapperNetworkAdapter networkAdapter) : base(options, networkAdapter)
    {
        networkAdapter.CommandRequestReceived += NetworkAdapter_CommandRequestReceived;
        networkAdapter.QueryRequestReceived += NetworkAdapter_QueryRequestReceived;
        networkAdapter.SchemaRequestReceived += NetworkAdapter_SchemaRequestReceived;
    }

    #region MANAGE INCOMING MESSAGES
    private void NetworkAdapter_SchemaRequestReceived(object? sender, NetworkAdapters.Events.SchemaReqReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NetworkAdapter_QueryRequestReceived(object? sender, NetworkAdapters.Events.QueryReqReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NetworkAdapter_CommandRequestReceived(object? sender, NetworkAdapters.Events.CommandReqReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region RECEIVED MESSAGE EVENTS
    public event EventHandler<CommandReqEventArgs>? CommandRequestReceived;
    public event EventHandler<QueryReqEventArgs>? QueryRequestReceived;
    public event EventHandler<SchemaReqEventArgs>? SchemaRequestReceived;
    #endregion

    #region SEND MESSAGES
    public Task<Result> SendCommandResponse(bool isSuccess, RemotePoint remotePoint, string outcomeDescription = "")
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendQueryResponse(TabularData queryResult, RemotePoint remotePoint, string? errorMessage = null, int blockNumber = 1, int totalBlocks = 1)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendSchemaResponse(DataSource schema, RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }
    #endregion
}
