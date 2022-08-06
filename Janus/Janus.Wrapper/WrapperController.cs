using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Wrapper.LocalCommanding;
using Janus.Wrapper.LocalQuerying;
using static FunctionalExtensions.Base.OptionExtensions;

namespace Janus.Wrapper;
public class WrapperController
    <TLocalQuery, TDeleteCommand, TInsertCommand, TUpdateCommand, TLocalSelection, TLocalJoining, TLocalProjection, TLocalData, TLocalMutation, TLocalInstantiation>
    : IComponentController
    where TLocalQuery : LocalQuery<TLocalSelection, TLocalJoining, TLocalProjection>
    where TDeleteCommand : LocalDelete<TLocalSelection>
    where TInsertCommand : LocalInsert<TLocalInstantiation>
    where TUpdateCommand : LocalUpdate<TLocalSelection, TLocalMutation>
{
    private readonly WrapperQueryManager<TLocalQuery, TLocalSelection, TLocalJoining, TLocalProjection, TLocalData> _queryManager;
    private readonly WrapperCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TLocalSelection, TLocalMutation, TLocalInstantiation> _commandManager;
    private readonly WrapperSchemaManager _schemaManager;
    private readonly WrapperCommunicationNode _communicationNode;

    public WrapperController(
        WrapperCommunicationNode communicationNode,
        WrapperQueryManager<TLocalQuery, TLocalSelection, TLocalJoining, TLocalProjection, TLocalData> queryManager,
        WrapperCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TLocalSelection, TLocalMutation, TLocalInstantiation> commandManager,
        WrapperSchemaManager schemaManager)
    {
        _communicationNode = communicationNode;
        _queryManager = queryManager;
        _commandManager = commandManager;
        _schemaManager = schemaManager;

        _communicationNode.CommandRequestReceived += CommunicationNode_CommandRequestReceived;
        _communicationNode.QueryRequestReceived += CommunicationNode_QueryRequestReceived;
        _communicationNode.SchemaRequestReceived += CommunicationNode_SchemaRequestReceived;
    }

    private void CommunicationNode_SchemaRequestReceived(object? sender, Communication.Nodes.Events.SchemaReqEventArgs e)
        => _schemaManager.GetCurrentSchema()
            .Map(dataSource => _communicationNode.SendSchemaResponse(e.ReceivedMessage.ExchangeId, dataSource, e.FromRemotePoint));

    private void CommunicationNode_QueryRequestReceived(object? sender, Communication.Nodes.Events.QueryReqEventArgs e)
    {
        var exchangeId = e.ReceivedMessage.ExchangeId;
        var fromPoint = e.FromRemotePoint;
        var query = e.ReceivedMessage.Query;

        _schemaManager.GetCurrentSchema()
            .Bind(dataSource => Task.FromResult(query.IsValidForDataSource(dataSource)))
            .Bind(result => _queryManager.RunQuery(query))
            .Match(
                queryResult => _communicationNode.SendQueryResponse(exchangeId, queryResult, fromPoint),
                message => _communicationNode.SendQueryResponse(exchangeId, null, fromPoint, $"Query execution error on {_communicationNode.Options.NodeId}:" + message)
            );
    }

    private void CommunicationNode_CommandRequestReceived(object? sender, Communication.Nodes.Events.CommandReqEventArgs e)
    {
        var exchangeId = e.ReceivedMessage.ExchangeId;
        var fromPoint = e.FromRemotePoint;
        var command = e.ReceivedMessage.Command;

        _schemaManager.GetCurrentSchema()
            .Bind(dataSource => Task.FromResult(command.IsValidForDataSource(dataSource)))
            .Bind(result => _commandManager.RunCommand(command))
            .Match(
                message => _communicationNode.SendCommandResponse(exchangeId, true, fromPoint, message),
                message => _communicationNode.SendCommandResponse(exchangeId, false, fromPoint, $"Command execution error on {_communicationNode.Options.NodeId}:" + message)
            );
    }

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
        => _communicationNode.RemotePoints;

    public async Task<Result<DataSource>> GetSchema()
        => await _schemaManager.GetCurrentSchema();

    public async Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
        => await _communicationNode.RegisterRemotePoint(new UndeterminedRemotePoint(address, port));

    public async Task<Result> RunCommand(BaseCommand command)
        => await _schemaManager
            .GetCurrentSchema()
            .Bind(dataSource => Task.FromResult(command.IsValidForDataSource(dataSource)))
            .Bind(async validity => await _commandManager.RunCommand(command));

    public async Task<Result<TabularData>> RunQuery(Query query)
        => await _schemaManager
            .GetCurrentSchema()
            .Bind(dataSource => Task.FromResult(query.IsValidForDataSource(dataSource)))
            .Bind(async validity => await _queryManager.RunQuery(query));

    public async Task<Result> SaveRegisteredRemotePoints(string filePath)
        => await ResultExtensions.AsResult(async () =>
        {
            await System.IO.File.WriteAllTextAsync(
                filePath,
                System.Text.Json.JsonSerializer.Serialize<IEnumerable<RemotePoint>>(_communicationNode.RemotePoints)
                );
            return true;
        });

    public async Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
        => await _communicationNode.SendHello(remotePoint);

    public async Task<Result<RemotePoint>> SendHello(string address, int port)
        => await _communicationNode.SendHello(new MediatorRemotePoint(address, port));

    public async Task<Result> UnregisterRemotePoint(RemotePoint remotePoint)
        => await _communicationNode.SendBye(remotePoint);
}
