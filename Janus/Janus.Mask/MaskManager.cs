﻿using Janus.Base;
using Janus.Base.Resulting;
using Janus.CommandLanguage;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mask.MaskedCommandModel;
using Janus.Mask.MaskedDataModel;
using Janus.Mask.MaskedQueryModel;
using Janus.Mask.MaskedSchemaModel;
using Janus.Mask.Persistence;
using Janus.Mask.Persistence.Models;
using Janus.QueryLanguage;

namespace Janus.Mask;
public abstract class MaskManager<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection, TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedMutation, TMaskedInstantiation, TMaskedSchema, TMaskedData, TMaskedDataItem>
    : IComponentManager
    where TMaskedQuery : MaskedQuery<TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection>
    where TMaskedDeleteCommand : MaskedDelete<TMaskedSelection>
    where TMaskedInsertCommand : MaskedInsert<TMaskedInstantiation>
    where TMaskedUpdateCommand : MaskedUpdate<TMaskedSelection, TMaskedMutation>
    where TMaskedSchema : MaskedDataSource
    where TMaskedData : MaskedData<TMaskedDataItem>

{
    protected readonly MaskQueryManager<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection, TMaskedSchema, TMaskedData, TMaskedDataItem> _queryManager;
    protected readonly MaskCommandManager<TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedSelection, TMaskedInstantiation, TMaskedMutation, TMaskedSchema> _commandManager;
    protected readonly MaskSchemaManager<TMaskedSchema> _schemaManager;
    protected readonly MaskCommunicationNode _communicationNode;
    protected readonly MaskPersistenceProvider _persistenceProvider;
    private readonly ILogger<MaskManager<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection, TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedMutation, TMaskedInstantiation, TMaskedSchema, TMaskedData, TMaskedDataItem>>? _logger;
    protected readonly MaskOptions _maskOptions;

    public MaskManager(MaskCommunicationNode communicationNode,
                       MaskQueryManager<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection, TMaskedSchema, TMaskedData, TMaskedDataItem> queryManager,
                       MaskCommandManager<TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedSelection, TMaskedInstantiation, TMaskedMutation, TMaskedSchema> commandManager,
                       MaskSchemaManager<TMaskedSchema> schemaManager,
                       MaskPersistenceProvider persistenceProvider,
                       MaskOptions maskOptions,
                       ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _queryManager = queryManager;
        _commandManager = commandManager;
        _schemaManager = schemaManager;
        _persistenceProvider = persistenceProvider;
        _maskOptions = maskOptions;
        _logger = logger?.ResolveLogger<MaskManager<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection, TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedMutation, TMaskedInstantiation, TMaskedSchema, TMaskedData, TMaskedDataItem>>();


        if (_maskOptions.EagerStartup)
        {
            InitiateEagerStartup().GetAwaiter().GetResult();
        }
    }

    /// <summary>
    /// Initiates eager startup operations on component initialization
    /// </summary>
    /// <returns>Result of startup operations</returns>
    private async Task<Result> InitiateEagerStartup()
    {
        var remotePointRegistrations = (await StartupRegisterRemotePoints()).Pass(r => _logger?.Info($"Registered startup remote points successfully: {r.Message}"), r => _logger?.Info($"Failed to register startup remote points: {r.Message}"));
        var schemaLoading = (await StartupLoadSchema()).Pass(r => _logger?.Info($"Loaded startup schemas successfully: {r.Message}"), r => _logger?.Info($"Failed to load startup schemas: {r.Message}"));
        var additionalOperations = (await AdditionalBaseStartupOperations()).Pass(r => _logger?.Info($"Additional operations completed successfully: {r.Message}"), r => _logger?.Info($"Additionaly operations failed: {r.Message}"));
        return remotePointRegistrations
            .Bind(_ => schemaLoading)
            .Bind(_ => additionalOperations);
    }

    /// <summary>
    /// Additional startup operations over the base class for specific mask manager kinds
    /// </summary>
    /// <returns>Result of additional operations</returns>
    protected async virtual Task<Result> AdditionalBaseStartupOperations()
        => await Task.FromResult(Results.OnSuccess("No additional operations required"));

    /// <summary>
    /// Tries to register the startup remote points in parallel
    /// </summary>
    private async Task<Result> StartupRegisterRemotePoints()
    {
        var remotePointRegistrations = _maskOptions.StartupRemotePoints
            .Map(async rp => await RegisterRemotePoint(rp))
            .Map(async result => (await result).Pass(r => _logger?.Info($"Registered startup remote point: {r.Data}"), r => _logger?.Info($"Failed to register startup remote point: {r.Message}")));

        var registrationResults = await Task.WhenAll(remotePointRegistrations);
        var registrationOperationResult = registrationResults.Fold(Results.OnSuccess(), (r, acc) => acc.Bind(_ => r ? Results.OnSuccess($"{_.Message};{r.Message}") : Results.OnFailure($"{_.Message};{r.Message}")));
        return registrationOperationResult;
    }

    /// <summary>
    /// Tries to load the startup schemas in parallel
    /// </summary>
    private async Task<Result> StartupLoadSchema()
    {
        if(string.IsNullOrWhiteSpace(_maskOptions.StartupNodeSchemaLoad))
        {
            _logger?.Info($"No startup schema load node id given");
            return Results.OnFailure($"No startup schema load node id given");
        }
        var remotePointForSchemaLoading = _communicationNode.RemotePoints.FirstOrDefault(rp => rp.NodeId.Equals(_maskOptions.StartupNodeSchemaLoad));

        if (remotePointForSchemaLoading == null)
        {
            _logger?.Info($"No remote point registered with node id: {_maskOptions.StartupNodeSchemaLoad}");

            return Results.OnFailure($"No remote point registered with node id: {_maskOptions.StartupNodeSchemaLoad}");
        }

        var schemaLoad =
            (await LoadSchemaFrom(remotePointForSchemaLoading))
            .Pass(r => _logger?.Info($"Loaded schema: {r.Data.Name}"), r => _logger?.Info($"Failed to load schema: {r.Message}"));


        var result = schemaLoad.Match(
            r => Results.OnSuccess(schemaLoad.Message),
            msg => Results.OnFailure(msg)
            );
        return result;
    }

    public Option<DataSource> GetCurrentSchema()
        => _schemaManager.CurrentOutputSchema;

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
        => _communicationNode.RemotePoints;

    public async Task<Result<RemotePoint>> RegisterRemotePoint(UndeterminedRemotePoint remotePoint)
        => await _communicationNode.RegisterRemotePoint(remotePoint);

    public async Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
        => await _communicationNode.RegisterRemotePoint(new UndeterminedRemotePoint(address, port));
    public async Task<Result<BaseCommand>> CreateCommand(string commandText)
        => CommandCompilation.CompileCommandFromScriptText(commandText);

    public async Task<Result> RunCommand(BaseCommand command)
        => await _commandManager.RunCommand(command);

    public async Task<Result<Query>> CreateQuery(string queryText)
    => QueryCompilation.CompileQueryFromScriptText(queryText);

    public async Task<Result<TabularData>> RunQuery(Query query)
        => await _queryManager.RunQuery(query);

    public async Task<Result<TabularData>> RunQueryOn(Query query, RemotePoint remotePoint)
        => await _queryManager.RunQueryOn(query, remotePoint);

    public async Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
        => await _communicationNode.SendHello(remotePoint);

    public async Task<Result<RemotePoint>> SendHello(string address, int port)
        => await _communicationNode.SendHello(new UndeterminedRemotePoint(address, port));

    public async Task<Result> UnregisterRemotePoint(RemotePoint remotePoint)
        => await _communicationNode.SendBye(remotePoint);

    public async Task<Result<DataSource>> GetSchemaFrom(RemotePoint remotePoint)
        => await _communicationNode.SendSchemaRequest(remotePoint);

    public async Task<Result<DataSource>> LoadSchemaFrom(RemotePoint remotePoint)
        => await _schemaManager.LoadSchema(remotePoint);

    public Result UnloadSchemaFrom(RemotePoint remotePoint)
        => _schemaManager.UnloadSchema(remotePoint);

    public Result UnloadSchema()
    => _schemaManager.UnloadSchema();

    public async Task<Result> PersistCurrentSchema(DataSource dataSource)
        => _persistenceProvider.DataSourceInfoPersistence.Insert(
            new Persistence.Models.DataSourceInfo(dataSource)
            );

    public async Task<Result<IEnumerable<DataSourceInfo>>> GetAllPersistedSchemas()
        => _persistenceProvider.DataSourceInfoPersistence.GetAll();

    public async Task<Result<DataSource>> LoadLatestSchemaFromPersistence()
        => _persistenceProvider.DataSourceInfoPersistence
            .GetLatest()
            .Map(dsInfo => dsInfo.InferredDataSource);

    public async Task<Result> DeleteSchema(string dataSourceVersion)
        => _persistenceProvider.DataSourceInfoPersistence.Delete(dataSourceVersion);

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
}
