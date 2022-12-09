using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;

namespace Janus.Mediator;
public sealed class MediatorManager : IComponentManager
{
    private readonly MediatorQueryManager _queryManager;
    private readonly MediatorCommandManager _commandManager;
    private readonly MediatorSchemaManager _schemaManager;
    private readonly MediatorCommunicationNode _communicationNode;

    public MediatorManager(MediatorCommunicationNode communicationNode, MediatorQueryManager queryManager, MediatorCommandManager commandManager, MediatorSchemaManager schemaManager)
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
    {

    }

    private void CommunicationNode_QueryRequestReceived(object? sender, Communication.Nodes.Events.QueryReqEventArgs e)
    {

    }

    private void CommunicationNode_CommandRequestReceived(object? sender, Communication.Nodes.Events.CommandReqEventArgs e)
    {

    }

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
        => _communicationNode.RemotePoints;

    public async Task<Result<DataSource>> GetSchema()
        => await Task.FromResult(
            _schemaManager.GetCurrentOutputSchema().Match(
                dataSource => Results.OnSuccess(dataSource),
                () => Results.OnFailure<DataSource>("No mediated schema generated")
            ));

    public async Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
        => await _communicationNode.RegisterRemotePoint(new UndeterminedRemotePoint(address, port));

    public async Task<Result> RunCommand(BaseCommand command)
        => Results.OnException(new NotImplementedException());

    public async Task<Result<TabularData>> RunQuery(Query query)
        => Results.OnException<TabularData>(new NotImplementedException());


    public async Task<Result> SaveRegisteredRemotePoints(string filePath)
        => await Results.AsResult(async () =>
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

    public IReadOnlyList<RemotePoint> LoadedSchemaRemotePoints
        => _schemaManager.DataSourceFromRemotePoint.Keys.ToList();

    public async Task<Result<DataSource>> IncludeInLoadedSchemas(RemotePoint remotePoint)
        => await _schemaManager.IncludeInLoadedSchemas(remotePoint);

    public Result ExcludeFromLoadedSchemas(RemotePoint remotePoint)
        => _schemaManager.ExcludeFromLoadedSchemas(remotePoint);

    public Result ClearRemotePointsFromSchemaInferrence()
        => _schemaManager.DataSourceFromRemotePoint.Keys.ToList()
            .Map(_schemaManager.ExcludeFromLoadedSchemas)
            .All(result => result);

    public async Task<IEnumerable<Result<DataSource>>> GetAvailableSchemas()
        => await _schemaManager.GetSchemasFromComponents();
}
