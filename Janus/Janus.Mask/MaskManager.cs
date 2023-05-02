using Janus.Base;
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


        RegisterStartupRemotePoints();
    }

    private async void RegisterStartupRemotePoints()
    {
        var regs = _maskOptions.StartupRemotePoints
            .Map(async rp => await RegisterRemotePoint(rp))
            .Map(async result => (await result).Pass(r => _logger?.Info($"Registered startup remote point: {r.Data}"), r => _logger?.Info($"Failed to register startup remote point: {r.Message}")));

        await Task.WhenAll(regs);
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
