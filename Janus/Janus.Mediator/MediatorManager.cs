using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
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

namespace Janus.Mediator;
public sealed class MediatorManager : IComponentManager
{
    private readonly MediatorQueryManager _queryManager;
    private readonly MediatorCommandManager _commandManager;
    private readonly MediatorSchemaManager _schemaManager;
    private readonly MediatorCommunicationNode _communicationNode;
    private readonly MediatorPersistenceProvider _persistenceProvider;
    private readonly ILogger<MediatorManager>? _logger;

    public MediatorManager(MediatorCommunicationNode communicationNode,
                           MediatorQueryManager queryManager,
                           MediatorCommandManager commandManager,
                           MediatorSchemaManager schemaManager,
                           MediatorPersistenceProvider persistenceProvider,
                           ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _queryManager = queryManager;
        _commandManager = commandManager;
        _schemaManager = schemaManager;
        _persistenceProvider = persistenceProvider;
        _logger = logger?.ResolveLogger<MediatorManager>();

        _communicationNode.CommandRequestReceived += CommunicationNode_CommandRequestReceived;
        _communicationNode.QueryRequestReceived += CommunicationNode_QueryRequestReceived;
        _communicationNode.SchemaRequestReceived += CommunicationNode_SchemaRequestReceived;
    }

    private async void CommunicationNode_SchemaRequestReceived(object? sender, Communication.Nodes.Events.SchemaReqEventArgs e)
    {
        (await _schemaManager.GetCurrentOutputSchema()
            .Match(
                async dataSource => await _communicationNode.SendSchemaResponse(e.ReceivedMessage.ExchangeId, dataSource, e.FromRemotePoint),
                async () => await Task.FromResult(Results.OnFailure("No mediated schema generated"))
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
                r => _logger?.Info($"Sent command response to {e.FromRemotePoint} after successful run of {Enum.GetName(e.ReceivedMessage.CommandReqType)} command {e.ReceivedMessage.Command.Name}."),
                r => _logger?.Info($"Sent command response to {e.FromRemotePoint} after failed command run.")
            ); ;
    }

    public async Task<Result<DataSource>> GetSchema()
        => await Task.FromResult(
            _schemaManager.GetCurrentOutputSchema().Match(
                dataSource => Results.OnSuccess(dataSource),
                () => Results.OnFailure<DataSource>("No mediated schema generated")
            ));

    public async Task<Result> RunCommand(BaseCommand command)
        => Results.OnException(new NotImplementedException());

    public async Task<Result<TabularData>> RunQuery(Query query)
        => Results.OnException<TabularData>(new NotImplementedException());

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


    public async Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
        => await _communicationNode.SendHello(remotePoint);

    public async Task<Result<RemotePoint>> SendHello(string address, int port)
        => await _communicationNode.SendHello(new UndeterminedRemotePoint(address, port));

    public async Task<Result<IEnumerable<RemotePoint>>> GetPersistedRemotePoints()
        => await Results.AsResult(async () => _persistenceProvider.RemotePointPersistence.GetAll());

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
        => _communicationNode.RemotePoints;

    public async Task<Result<RemotePoint>> RegisterRemotePoint(UndeterminedRemotePoint remotePoint)
        => await _communicationNode.RegisterRemotePoint(remotePoint);

    public async Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
        => await _communicationNode.RegisterRemotePoint(new UndeterminedRemotePoint(address, port));

    public async Task<Result> UnregisterRemotePoint(RemotePoint remotePoint)
        => await _communicationNode.SendBye(remotePoint);

    public IReadOnlyList<RemotePoint> LoadedSchemaRemotePoints
        => _schemaManager.DataSourceFromRemotePoint.Keys.ToList();

    public async Task<Result<DataSource>> LoadSchemaFrom(RemotePoint remotePoint)
        => await _schemaManager.LoadSchema(remotePoint);

    public Result UnloadSchema(string dataSourceName)
        => _schemaManager.UnloadSchema(dataSourceName);
    public Result UnloadSchemaFrom(RemotePoint remotePoint)
        => _schemaManager.UnloadSchema(remotePoint);

    public Result UnloadAllSchemas()
        => _schemaManager.DataSourceFromRemotePoint.Keys.ToList()
            .Map(_schemaManager.UnloadSchema)
            .All(result => result);

    public async Task<Dictionary<RemotePoint, Result<DataSource>>> GetAvailableSchemas()
        => await _schemaManager.GetSchemasFromComponents();

    public async Task<Result<DataSource>> LoadMediatedSchemaFromPersistence()
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
            targetRemotePoints
                .Map(async rp => await _schemaManager.LoadSchema(rp))
                .Fold(Results.OnSuccess(), (schema, finalResult) => finalResult.Bind(_ => _));
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

    public Result<DataSourceMediation> CreateDataSourceMediation(string mediationScript)
        => Results.AsResult(() =>
        {
            var loadedDataSources = _schemaManager.LoadedDataSources;

            return MediationFactory.CreateMediationFromScriptText(mediationScript, loadedDataSources);
        });

    public async Task<Result<DataSource>> ApplyMediation(DataSourceMediation dataSourceMediation)
        => await Results.AsResult(async () =>
        {
            return await _schemaManager.MediateLoadedSchemas(dataSourceMediation);
        });
}
