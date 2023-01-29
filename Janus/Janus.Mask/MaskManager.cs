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
using Janus.Mask.Persistence;
using Janus.Mask.Persistence.Models;
using Janus.QueryLanguage;

namespace Janus.Mask;
public class MaskManager : IComponentManager
{
    private readonly MaskQueryManager _queryManager;
    private readonly MaskCommandManager _commandManager;
    private readonly MaskSchemaManager _schemaManager;
    private readonly MaskCommunicationNode _communicationNode;
    private readonly MaskPersistenceProvider _persistenceProvider;
    private readonly ILogger<MaskManager>? _logger;
    private readonly MaskOptions _maskOptions;

    public MaskManager(MaskCommunicationNode communicationNode,
                       MaskQueryManager queryManager,
                       MaskCommandManager commandManager,
                       MaskSchemaManager schemaManager,
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
        _logger = logger?.ResolveLogger<MaskManager>();


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
        => _schemaManager.GetCurrentOutputSchema();

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
