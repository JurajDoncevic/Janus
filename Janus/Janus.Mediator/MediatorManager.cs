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

/// <summary>
/// Mediator component manager
/// </summary>
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

    /// <summary>
    /// Tries to registers the startup remote points in parallel
    /// </summary>
    private async void RegisterStartupRemotePoints()
    {
        var regs = _mediatorOptions.StartupRemotePoints
            .Map(async rp => await RegisterRemotePoint(rp))
            .Map(async result => (await result).Pass(r => _logger?.Info($"Registered startup remote point: {r.Data}"), r => _logger?.Info($"Failed to register startup remote point: {r.Message}")));

        await Task.WhenAll(regs);
    }

    #region RECEIVED MESSAGE HANDLERS
    private async void CommunicationNode_SchemaRequestReceived(object? sender, Communication.Nodes.Events.SchemaReqEventArgs e)
    {
        (await _schemaManager.CurrentOutputSchema
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

    #endregion

    #region COMMANDING
    /// <summary>
    /// Creates a command from the given command text in the command language
    /// </summary>
    /// <param name="commandText">Command text</param>
    /// <returns>Result of command creation</returns>
    public async Task<Result<BaseCommand>> CreateCommand(string commandText)
        => CommandCompilation.CompileCommandFromScriptText(commandText);

    public async Task<Result> RunCommand(BaseCommand command)
        => await _commandManager.RunCommand(command, _schemaManager);
    #endregion

    #region QUERYING
    /// <summary>
    /// Creates a query from the given query text in the query language
    /// </summary>
    /// <param name="queryText">Query text</param>
    /// <returns>Result of query creation</returns>
    public async Task<Result<Query>> CreateQuery(string queryText)
    => QueryCompilation.CompileQueryFromScriptText(queryText);

    public async Task<Result<TabularData>> RunQuery(Query query)
        => await _queryManager.RunQuery(query, _schemaManager);

    /// <summary>
    /// Runs a query on the given remote point
    /// </summary>
    /// <param name="query"></param>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public async Task<Result<TabularData>> RunQueryOn(Query query, RemotePoint remotePoint)
        => await _queryManager.RunQueryOn(query, remotePoint);
    #endregion

    #region PERSISTENCE
    /// <summary>
    /// Gets the persisted remote points
    /// </summary>
    /// <returns>Result of remote points</returns>
    public async Task<Result<IEnumerable<RemotePoint>>> GetPersistedRemotePoints()
        => await Results.AsResult(async () => _persistenceProvider.RemotePointPersistence.GetAll());

    /// <summary>
    /// Persists a remote point
    /// </summary>
    /// <param name="remotePoint">Remote point to persist</param>
    /// <returns>Operation result</returns>
    public async Task<Result> PersistRemotePoint(RemotePoint remotePoint)
        => await Results.AsResult(async () =>
        {
            var inserting =
                _persistenceProvider.RemotePointPersistence.Insert(remotePoint);

            return inserting;
        });

    /// <summary>
    /// Deletes a remote point from persistence
    /// </summary>
    /// <param name="nodeId">Remote point node id</param>
    /// <returns>Result of deletion</returns>
    public async Task<Result> DeleteRemotePoint(string nodeId)
        => await Results.AsResult(async () =>
        {
            var deletion =
                _persistenceProvider.RemotePointPersistence.Delete(nodeId);
        
            return deletion;
        });

    /// <summary>
    /// Persists multiple remote points
    /// </summary>
    /// <param name="remotePoints">Remote points to persist</param>
    /// <returns>Operation result</returns>
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

    /// <summary>
    /// Persists the current mediated schema, together with the mediation that created it.
    /// </summary>
    /// <param name="currentDataSource">Current data source</param>
    /// <param name="dataSourceMediation">Current data source mediation that created the current data source</param>
    /// <param name="loadedDataSources">Loaded data sources required to implement the mediation</param>
    /// <param name="createdOn">DateTime of mediation creation</param>
    /// <returns>Operation result</returns>
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

    /// <summary>
    /// Deletes a mediated schema from persistence 
    /// </summary>
    /// <param name="version">Version of the schema</param>
    /// <returns>Operation result</returns>
    public async Task<Result> DeleteMediatedSchema(string version)
        => await Results.AsResult(async () =>
        {
            var deletion = _persistenceProvider.DataSourceInfoPersistence.Delete(version);

            return deletion;
        });

    /// <summary>
    /// Gets all persisted (mediated) schemas
    /// </summary>
    /// <returns>Persisted data source schemas</returns>
    public async Task<Result<IEnumerable<Persistence.Models.DataSourceInfo>>> GetAllPersistedSchemas()
        => await Results.AsResult(async () =>
        {
            var persistedSchemas = _persistenceProvider.DataSourceInfoPersistence.GetAll();
            
            return persistedSchemas;
        });

    /// <summary>
    /// Loads the latest persisted mediated schema
    /// </summary>
    /// <returns>Latets mediated data source schema</returns>
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
    #endregion

    #region COMMUNICATION
    public async Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
    => await _communicationNode.SendHello(remotePoint);

    public async Task<Result<RemotePoint>> SendHello(string address, int port)
        => await _communicationNode.SendHello(new UndeterminedRemotePoint(address, port));

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
        => _communicationNode.RemotePoints;

    public async Task<Result<RemotePoint>> RegisterRemotePoint(UndeterminedRemotePoint remotePoint)
        => await _communicationNode.RegisterRemotePoint(remotePoint);

    public async Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
        => await _communicationNode.RegisterRemotePoint(new UndeterminedRemotePoint(address, port));

    public async Task<Result> UnregisterRemotePoint(RemotePoint remotePoint)
        => await _communicationNode.SendBye(remotePoint);
    #endregion

    #region SCHEMA MANAGEMENT AND MEDIATION
    public Option<DataSource> GetCurrentSchema()
        => _schemaManager.CurrentOutputSchema;

    /// <summary>
    /// Remote points of currently loaded data sources
    /// </summary>
    public IReadOnlyList<RemotePoint> LoadedSchemaRemotePoints
        => _schemaManager.LoadedDataSourceFromRemotePoint.Keys.ToList();

    /// <summary>
    /// Gets a schema from a remote point
    /// </summary>
    /// <param name="remotePoint">Remote point</param>
    /// <returns>Schema acquisition result</returns>
    public async Task<Result<DataSource>> GetSchemaFrom(RemotePoint remotePoint)
        => await _schemaManager.GetSchemaFrom(remotePoint);

    /// <summary>
    /// Gets loaded schemas by their remote points 
    /// </summary>
    /// <returns>Dictionary of remote points and data source schemas</returns>
    public IReadOnlyDictionary<RemotePoint, DataSource> GetLoadedSchemas()
        => _schemaManager.LoadedDataSourceFromRemotePoint;

    /// <summary>
    /// Loads a schema from a remote point
    /// </summary>
    /// <param name="remotePoint">Remote point</param>
    /// <returns>Loaded data source schema</returns>
    public async Task<Result<DataSource>> LoadSchemaFrom(RemotePoint remotePoint)
        => await _schemaManager.LoadSchema(remotePoint);

    /// <summary>
    /// Unloads a schema by the data source name
    /// </summary>
    /// <param name="dataSourceName">Schema data source name</param>
    /// <returns>Operation result</returns>
    public Result UnloadSchema(string dataSourceName)
        => _schemaManager.UnloadSchema(dataSourceName);

    /// <summary>
    /// Unloads a schema coming from the given remote point
    /// </summary>
    /// <param name="remotePoint">Source remote point of the data source schema</param>
    /// <returns>Operation result</returns>
    public Result UnloadSchemaFrom(RemotePoint remotePoint)
        => _schemaManager.UnloadSchema(remotePoint);

    /// <summary>
    /// Unloads all schemas
    /// </summary>
    /// <returns>Operation result</returns>
    public Result UnloadAllSchemas()
        => _schemaManager.LoadedDataSourceFromRemotePoint.Keys.ToList()
            .Map(_schemaManager.UnloadSchema)
            .All(result => result);

    /// <summary>
    /// Gets all available schemas from registered remote points
    /// </summary>
    /// <returns>Dictionary of registered remote points and schema acquisition outcomes</returns>
    public async Task<Dictionary<RemotePoint, Result<DataSource>>> GetAvailableSchemas()
        => await _schemaManager.GetSchemasFromComponents();

    /// <summary>
    /// Creates a data source mediation from a mediation script written in the mediation language. 
    /// Doesn't apply the mediation to the component!
    /// </summary>
    /// <param name="mediationScript">Mediation script text</param>
    /// <returns>Result of data source mediation creation</returns>
    public Result<DataSourceMediation> CreateDataSourceMediation(string mediationScript)
        => Results.AsResult(() =>
        {
            var loadedDataSources = _schemaManager.LoadedDataSources;

            return MediationCompilation.CompileMediationFromScriptText(mediationScript, loadedDataSources);
        });

    /// <summary>
    /// Applies a mediation to the component
    /// </summary>
    /// <param name="dataSourceMediation"></param>
    /// <returns>Mediation result</returns>
    public async Task<Result<DataSource>> ApplyMediation(DataSourceMediation dataSourceMediation)
        => await Results.AsResult(async () =>
        {
            return await _schemaManager.MediateLoadedSchemas(dataSourceMediation);
        });

    /// <summary>
    /// Gets the current schema mediation
    /// </summary>
    /// <returns>Optional data source mediation object</returns>
    public Option<DataSourceMediation> GetCurrentSchemaMediation()
        => _schemaManager.CurrentMediation;
    #endregion
}
