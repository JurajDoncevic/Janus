using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.CommandLanguage;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mediation.SchemaMediationModels;
using Janus.MediationLanguage;
using Janus.Mediator.Persistence;
using Janus.QueryLanguage;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Janus.Mediator;
public sealed class MediatorManager : IComponentManager
{
    private readonly MediatorQueryManager _queryManager;
    private readonly MediatorCommandManager _commandManager;
    private readonly MediatorSchemaManager _schemaManager;
    private readonly MediatorCommunicationNode _communicationNode;
    private readonly MediatorPersistenceProvider _persistenceProvider;
    private readonly ILogger<MediatorManager>? _logger;
    private readonly MediatorOptions _mediatorOptions;

    public MediatorManager(MediatorCommunicationNode communicationNode,
                           MediatorQueryManager queryManager,
                           MediatorCommandManager commandManager,
                           MediatorSchemaManager schemaManager,
                           MediatorPersistenceProvider persistenceProvider,
                           MediatorOptions mediatorOptions,
                           ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _queryManager = queryManager;
        _commandManager = commandManager;
        _schemaManager = schemaManager;
        _persistenceProvider = persistenceProvider;
        _mediatorOptions = mediatorOptions;
        _logger = logger?.ResolveLogger<MediatorManager>();

        _communicationNode.CommandRequestReceived += CommunicationNode_CommandRequestReceived;
        _communicationNode.QueryRequestReceived += CommunicationNode_QueryRequestReceived;
        _communicationNode.SchemaRequestReceived += CommunicationNode_SchemaRequestReceived;

        RegisterStartupRemotePoints();
    }

    private async void RegisterStartupRemotePoints()
    {
        var regs = _mediatorOptions.StartupRemotePoints
            .Map(async rp => await RegisterRemotePoint(rp))
            .Map(async result => (await result).Pass(r => _logger?.Info($"Registered startup remote point: {r.Data}"), r => _logger?.Info($"Failed to register startup remote point: {r.Message}")));

        await Task.WhenAll(regs);
    }

    private async void CommunicationNode_SchemaRequestReceived(object? sender, Communication.Nodes.Events.SchemaReqEventArgs e)
    {
        (await _schemaManager.GetCurrentOutputSchema()
            .Match(
                async dataSource => await _communicationNode.SendSchemaResponse(e.ReceivedMessage.ExchangeId, dataSource, e.FromRemotePoint),
                async () => await _communicationNode.SendSchemaResponse(e.ReceivedMessage.ExchangeId, null, e.FromRemotePoint, "No mediated schema generated")
            )).Pass(
                r => _logger?.Info($"Sent current schema to {e.FromRemotePoint} on request"),
                r => _logger?.Info($"Failed to respond to schema request from {e.FromRemotePoint} due to: {r.Message}")
            );
    }

    private async void CommunicationNode_QueryRequestReceived(object? sender, Communication.Nodes.Events.QueryReqEventArgs e)
    {
        (await (await _queryManager.RunQuery(e.ReceivedMessage.Query, _schemaManager))
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
        (await (await _commandManager.RunCommand(e.ReceivedMessage.Command, _schemaManager))
            .Match(
                message => _communicationNode.SendCommandResponse(e.ReceivedMessage.ExchangeId, true, e.FromRemotePoint, message),
                message => _communicationNode.SendCommandResponse(e.ReceivedMessage.ExchangeId, false, e.FromRemotePoint, message),
                message => _communicationNode.SendCommandResponse(e.ReceivedMessage.ExchangeId, false, e.FromRemotePoint, message)
            )).Pass(
                r => _logger?.Info($"Sent command response to {e.FromRemotePoint} after successful run of {e.ReceivedMessage.CommandReqType} command {e.ReceivedMessage.Command.Name}."),
                r => _logger?.Info($"Sent command response to {e.FromRemotePoint} after failed command run.")
            );
    }

    public Option<DataSource> GetCurrentSchema()
        => _schemaManager.GetCurrentOutputSchema();

    public async Task<Result<BaseCommand>> CreateCommand(string commandText)
        => CommandCompilation.CompileCommandFromScriptText(commandText);

    public async Task<Result> RunCommand(BaseCommand command)
        => await _commandManager.RunCommand(command, _schemaManager);

    public async Task<Result<Query>> CreateQuery(string queryText)
    => QueryCompilation.CompileQueryFromScriptText(queryText);

    public async Task<Result<TabularData>> RunQuery(Query query)
        => await _queryManager.RunQuery(query, _schemaManager);

    public async Task<Result<TabularData>> RunQueryOn(Query query, RemotePoint remotePoint)
        => await _queryManager.RunQueryOn(query, remotePoint);

    public async Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
        => await _communicationNode.SendHello(remotePoint);

    public async Task<Result<RemotePoint>> SendHello(string address, int port)
        => await _communicationNode.SendHello(new UndeterminedRemotePoint(address, port));

    public async Task<Result<IEnumerable<RemotePoint>>> GetPersistedRemotePoints()
        => await Results.AsResult(async () => _persistenceProvider.RemotePointPersistence.GetAll());

    public async Task<Result> PersistRemotePoint(RemotePoint remotePoint)
        => await Results.AsResult(async () =>
        {
            var inserting =
                _persistenceProvider.RemotePointPersistence.Insert(remotePoint);

            return inserting;
        });

    public async Task<Result> DeleteRemotePoint(string nodeId)
        => await Results.AsResult(async () =>
        {
            var deletion =
                _persistenceProvider.RemotePointPersistence.Delete(nodeId);
        
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

    public async Task<Result> PersistCurrentMediatedSchema(DataSource currentDataSource, DataSourceMediation dataSourceMediation, Dictionary<RemotePoint, DataSource> loadedDataSources, DateTime? createdOn = null)
        => await Results.AsResult(async () =>
        {
            var insertion =
                _persistenceProvider
                .DataSourceInfoPersistence.Insert(new Persistence.Models.DataSourceInfo(
                    currentDataSource,
                    dataSourceMediation.ToMediationScript(),
                    loadedDataSources,
                    createdOn
                    ));

            return insertion;
        });

    public async Task<Result> DeleteMediatedSchema(string version)
        => await Results.AsResult(async () =>
        {
            var deletion = _persistenceProvider.DataSourceInfoPersistence.Delete(version);

            return deletion;
        });

    public async Task<Result<IEnumerable<Persistence.Models.DataSourceInfo>>> GetAllPersistedSchemas()
        => await Results.AsResult(async () =>
        {
            var persistedSchemas = _persistenceProvider.DataSourceInfoPersistence.GetAll();
            
            return persistedSchemas;
        });

    public async Task<Result<DataSource>> LoadLatestMediatedSchemaFromPersistence()
        => await Results.AsResult(async () =>
        {
            var dataSourceInfoAcquisition = _persistenceProvider.DataSourceInfoPersistence.GetLatest();
            if (!dataSourceInfoAcquisition)
            {
                return Results.OnFailure<DataSource>("Couldn't load data source info from persistence");
            }
            var dataSourceInfo = dataSourceInfoAcquisition.Data;

            var targetRemotePoints = dataSourceInfo.LoadedDataSources.Keys;

            _schemaManager.UnloadSchemas();

            var schemaLoading =
                await targetRemotePoints
                    .Map(async rp => await _schemaManager.LoadSchema(rp))
                    .Aggregate((r1, r2) => r1.Bind(_ => r2));
            if (!schemaLoading)
            {
                return Results.OnFailure<DataSource>($"Couldn't load schemas from remote points: {schemaLoading.Message}");
            }

            var mediationCreation = CreateDataSourceMediation(dataSourceInfo.MediationScript);
            if (!mediationCreation)
            {
                return Results.OnFailure<DataSource>($"Couldn't create mediation from script: {mediationCreation.Message}");
            }

            var mediation = await _schemaManager.MediateLoadedSchemas(mediationCreation.Data);
            if (!mediation)
            {
                return Results.OnFailure<DataSource>($"Couldn't mediate loaded schemas: {mediation.Message}");
            }

            return mediation.Data;
        });

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
        => _communicationNode.RemotePoints;

    public async Task<Result<RemotePoint>> RegisterRemotePoint(UndeterminedRemotePoint remotePoint)
        => await _communicationNode.RegisterRemotePoint(remotePoint);

    public async Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
        => await _communicationNode.RegisterRemotePoint(new UndeterminedRemotePoint(address, port));

    public async Task<Result> UnregisterRemotePoint(RemotePoint remotePoint)
        => await _communicationNode.SendBye(remotePoint);

    public IReadOnlyList<RemotePoint> LoadedSchemaRemotePoints
        => _schemaManager.LoadedDataSourceFromRemotePoint.Keys.ToList();

    public async Task<Result<DataSource>> GetSchemaFrom(RemotePoint remotePoint)
        => await _schemaManager.GetSchemaFrom(remotePoint);

    public IReadOnlyDictionary<RemotePoint, DataSource> GetLoadedSchemas()
        => _schemaManager.LoadedDataSourceFromRemotePoint;

    public async Task<Result<DataSource>> LoadSchemaFrom(RemotePoint remotePoint)
        => await _schemaManager.LoadSchema(remotePoint);

    public Result UnloadSchema(string dataSourceName)
        => _schemaManager.UnloadSchema(dataSourceName);
    public Result UnloadSchemaFrom(RemotePoint remotePoint)
        => _schemaManager.UnloadSchema(remotePoint);

    public Result UnloadAllSchemas()
        => _schemaManager.LoadedDataSourceFromRemotePoint.Keys.ToList()
            .Map(_schemaManager.UnloadSchema)
            .All(result => result);

    public async Task<Dictionary<RemotePoint, Result<DataSource>>> GetAvailableSchemas()
        => await _schemaManager.GetSchemasFromComponents();

    public Result<DataSourceMediation> CreateDataSourceMediation(string mediationScript)
        => Results.AsResult(() =>
        {
            var loadedDataSources = _schemaManager.LoadedDataSources;

            return MediationCompilation.CompileMediationFromScriptText(mediationScript, loadedDataSources);
        });

    public async Task<Result<DataSource>> ApplyMediation(DataSourceMediation dataSourceMediation)
        => await Results.AsResult(async () =>
        {
            return await _schemaManager.MediateLoadedSchemas(dataSourceMediation);
        });

    public Option<DataSourceMediation> GetCurrentSchemaMediation()
        => _schemaManager.CurrentMediation;
}
