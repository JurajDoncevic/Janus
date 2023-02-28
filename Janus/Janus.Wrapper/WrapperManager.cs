using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.CommandLanguage;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SchemaModels.Building;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.QueryLanguage;
using Janus.Wrapper.LocalCommanding;
using Janus.Wrapper.LocalQuerying;
using Janus.Wrapper.Persistence;
using Janus.Wrapper.Persistence.Models;
using static FunctionalExtensions.Base.OptionExtensions;

namespace Janus.Wrapper;
/// <summary>
/// Manager for a wrapper component
/// </summary>
/// <typeparam name="TLocalQuery"></typeparam>
/// <typeparam name="TDeleteCommand"></typeparam>
/// <typeparam name="TInsertCommand"></typeparam>
/// <typeparam name="TUpdateCommand"></typeparam>
/// <typeparam name="TLocalSelection"></typeparam>
/// <typeparam name="TLocalJoining"></typeparam>
/// <typeparam name="TLocalProjection"></typeparam>
/// <typeparam name="TLocalData"></typeparam>
/// <typeparam name="TLocalMutation"></typeparam>
/// <typeparam name="TLocalInstantiation"></typeparam>
public abstract class WrapperManager
    <TLocalQuery, TDeleteCommand, TInsertCommand, TUpdateCommand, TLocalSelection, TLocalJoining, TLocalProjection, TLocalData, TLocalMutation, TLocalInstantiation>
    : IComponentManager
    where TLocalQuery : LocalQuery<TLocalSelection, TLocalJoining, TLocalProjection>
    where TDeleteCommand : LocalDelete<TLocalSelection>
    where TInsertCommand : LocalInsert<TLocalInstantiation>
    where TUpdateCommand : LocalUpdate<TLocalSelection, TLocalMutation>
{
    private readonly WrapperQueryManager<TLocalQuery, TLocalSelection, TLocalJoining, TLocalProjection, TLocalData> _queryManager;
    private readonly WrapperCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TLocalSelection, TLocalMutation, TLocalInstantiation> _commandManager;
    private readonly WrapperSchemaManager _schemaManager;
    private readonly WrapperCommunicationNode _communicationNode;
    private readonly WrapperPersistenceProvider _persistenceProvider;
    private readonly WrapperOptions _wrapperOptions;
    private readonly ILogger<WrapperManager<TLocalQuery, TDeleteCommand, TInsertCommand, TUpdateCommand, TLocalSelection, TLocalJoining, TLocalProjection, TLocalData, TLocalMutation, TLocalInstantiation>>? _logger;

    public WrapperManager(
        WrapperCommunicationNode communicationNode,
        WrapperQueryManager<TLocalQuery, TLocalSelection, TLocalJoining, TLocalProjection, TLocalData> queryManager,
        WrapperCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TLocalSelection, TLocalMutation, TLocalInstantiation> commandManager,
        WrapperSchemaManager schemaManager,
        WrapperPersistenceProvider persistenceProvider,
        WrapperOptions wrapperOptions,
        ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _queryManager = queryManager;
        _commandManager = commandManager;
        _schemaManager = schemaManager;
        _persistenceProvider = persistenceProvider;
        _wrapperOptions = wrapperOptions;
        _logger = logger?.ResolveLogger<WrapperManager<TLocalQuery, TDeleteCommand, TInsertCommand, TUpdateCommand, TLocalSelection, TLocalJoining, TLocalProjection, TLocalData, TLocalMutation, TLocalInstantiation>>();

        _communicationNode.CommandRequestReceived += CommunicationNode_CommandRequestReceived;
        _communicationNode.QueryRequestReceived += CommunicationNode_QueryRequestReceived;
        _communicationNode.SchemaRequestReceived += CommunicationNode_SchemaRequestReceived;
    }

    private void CommunicationNode_SchemaRequestReceived(object? sender, Communication.Nodes.Events.SchemaReqEventArgs e)
        => _schemaManager.CurrentOutputSchema
            .Match(
                dataSource => _communicationNode.SendSchemaResponse(e.ReceivedMessage.ExchangeId, dataSource, e.FromRemotePoint),
                () => _communicationNode.SendSchemaResponse(e.ReceivedMessage.ExchangeId, null, e.FromRemotePoint, "No schema generated")
                );

    private async void CommunicationNode_QueryRequestReceived(object? sender, Communication.Nodes.Events.QueryReqEventArgs e)
    {        
        (await (await RunQuery(e.ReceivedMessage.Query))
            .Match(
                tabularData => _communicationNode.SendQueryResponse(e.ReceivedMessage.ExchangeId, tabularData, e.FromRemotePoint),
                message => _communicationNode.SendQueryResponse(e.ReceivedMessage.ExchangeId, null, e.FromRemotePoint, message),
                message => _communicationNode.SendQueryResponse(e.ReceivedMessage.ExchangeId, null, e.FromRemotePoint, message)
            )).Pass(
                r => _logger?.Info($"Sent query response to {e.FromRemotePoint} after successful run of query {e.ReceivedMessage.Query.Name}."),
                r => _logger?.Info($"Sent query response to {e.FromRemotePoint} after failed query run.")
            );
    }

    private async void CommunicationNode_CommandRequestReceived(object? sender, Communication.Nodes.Events.CommandReqEventArgs e)
    {
        (await (await RunCommand(e.ReceivedMessage.Command))
            .Match(
                message => _communicationNode.SendCommandResponse(e.ReceivedMessage.ExchangeId, true, e.FromRemotePoint, message),
                message => _communicationNode.SendCommandResponse(e.ReceivedMessage.ExchangeId, false, e.FromRemotePoint, message),
                message => _communicationNode.SendCommandResponse(e.ReceivedMessage.ExchangeId, false, e.FromRemotePoint, message)
            )).Pass(
                r => _logger?.Info($"Sent command response to {e.FromRemotePoint} after successful run of {Enum.GetName(e.ReceivedMessage.CommandReqType)} command {e.ReceivedMessage.Command.Name}."),
                r => _logger?.Info($"Sent command response to {e.FromRemotePoint} after failed command run.")
            );
    }

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
        => _communicationNode.RemotePoints;

    public async Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
        => await _communicationNode.SendHello(remotePoint);

    public async Task<Result<RemotePoint>> SendHello(string address, int port)
        => await _communicationNode.SendHello(new MediatorRemotePoint(address, port));

    public async Task<Result<RemotePoint>> RegisterRemotePoint(UndeterminedRemotePoint remotePoint)
        => await _communicationNode.RegisterRemotePoint(remotePoint);

    public async Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
        => await _communicationNode.RegisterRemotePoint(new UndeterminedRemotePoint(address, port));

    public async Task<Result> UnregisterRemotePoint(RemotePoint remotePoint)
        => await _communicationNode.SendBye(remotePoint);

    public async Task<Result<IEnumerable<RemotePoint>>> GetPersistedRemotePoints()
        => await Results.AsResult(async () => _persistenceProvider.RemotePointPersistence.GetAll());
    
    public async Task<Result> PersistRemotePoint(RemotePoint remotePoint)
        => await Results.AsResult(async () =>
        {
            var insertion = _persistenceProvider.RemotePointPersistence.Insert(remotePoint);

            return insertion;
        });

    public async Task<Result> DeleteRemotePoint(string nodeId)
        => await Results.AsResult(async () =>
        {
            var deletion = _persistenceProvider.RemotePointPersistence.Delete(nodeId);

            return deletion;
        });

    public async Task<Result> PersistRemotePoints(IEnumerable<RemotePoint> remotePoints)
        => await Results.AsResult(async () =>
        {
            // remove remote points from persistence not in given enumerable
            var removal =
            _persistenceProvider.RemotePointPersistence.GetAll()
                .Map(rps => rps.Where(rp => !remotePoints.Contains(rp)))
                .Map(rps => rps.Fold(Results.OnSuccess(), (rp, result) => result.Bind(r => _persistenceProvider.RemotePointPersistence.Delete(rp.NodeId))));

            // insert non-persisted remote points
            var insertion =
            remotePoints
                .Where(rp => !_persistenceProvider.RemotePointPersistence.Exists(rp.NodeId))
                .Fold(Results.OnSuccess(), (rp, result) => result.Bind(r => _persistenceProvider.RemotePointPersistence.Insert(rp)));

            return insertion && removal;
        });

    public async Task<Result<IEnumerable<Persistence.Models.DataSourceInfo>>> GetPersistedSchemas()
        => await Results.AsResult(async () => _persistenceProvider.DataSourceInfoPersistence.GetAll());

    public async Task<Result> PersistSchema(DataSource dataSource)
        => await Results.AsResult(async () =>
        {
            var dataSourceInfo = new DataSourceInfo(dataSource);
            var insertion =
                _persistenceProvider.DataSourceInfoPersistence.Insert(dataSourceInfo);

            return insertion;
        });

    public async Task<Result> DeletePersistedSchema(string schemaVersion)
        => await Results.AsResult(async () =>
        {
            var deletion =
                _persistenceProvider.DataSourceInfoPersistence.Delete(schemaVersion);

            return deletion;
        });

    public Option<DataSource> GetCurrentSchema()
        => _schemaManager.CurrentOutputSchema;

    public async Task<Result<DataSource>> GenerateSchema()
        => await _schemaManager.ReloadOutputSchema();

    public async Task<Result<BaseCommand>> CreateCommand(string commandText)
        => CommandCompilation.CompileCommandFromScriptText(commandText);

    public async Task<Result<Query>> CreateQuery(string queryText)
        => QueryCompilation.CompileQueryFromScriptText(queryText);


    public async Task<Result> RunCommand(BaseCommand command)
        => !_wrapperOptions.AllowsCommands
            ? Results.OnFailure("This wrapper does not allow running commands")
            : await _schemaManager
                .CurrentOutputSchema
                .Match(
                    async dataSource => await Task.FromResult(command.IsValidForDataSource(dataSource))
                                                  .Bind(async validity => await _commandManager.RunCommand(command)),
                    async () => Results.OnFailure("No schema generated")
                );

    public async Task<Result<TabularData>> RunQuery(Query query)
        => await _schemaManager
            .CurrentOutputSchema
            .Match(
                async dataSource => await Task.FromResult(query.IsValidForDataSource(dataSource))
                                              .Bind(async validity => await _queryManager.RunQuery(query)),
                async () => Results.OnFailure<TabularData>("No schema generated")
            );

}
