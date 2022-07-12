using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Utils.Logging;

namespace Janus.Wrapper.Core;
public class WrapperController<TQueryDestination, TSelectionDestination, TJoiningDestination, TProjectionDestination> : IComponentController
{
    private readonly WrapperCommunicationNode _communicationNode;
    private readonly WrapperCommandManager _commandManager;
    private readonly WrapperQueryManager<TQueryDestination, TSelectionDestination, TJoiningDestination, TProjectionDestination> _queryManager;
    private readonly WrapperSchemaManager _schemaManager;
    private readonly ILogger<WrapperController<TQueryDestination, TSelectionDestination, TJoiningDestination, TProjectionDestination>>? _logger;

    public WrapperController(WrapperCommunicationNode communicationNode, WrapperCommandManager commandManager, WrapperQueryManager<TQueryDestination, TSelectionDestination, TJoiningDestination, TProjectionDestination> queryManager, WrapperSchemaManager schemaManager, ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _commandManager = commandManager;
        _queryManager = queryManager;
        _schemaManager = schemaManager;
        _logger = logger?.ResolveLogger<WrapperController<TQueryDestination, TSelectionDestination, TJoiningDestination, TProjectionDestination>>();
    }

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
        => _communicationNode.RemotePoints;

    public async Task<Result<DataSource>> GetSchema()
        => await _schemaManager.GetCurrentSchema();

    public async Task<Result<DataSource>> ReloadSchema()
        => await _schemaManager.ReloadSchema();

    public async Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
        => await _communicationNode.RegisterRemotePoint(new UndeterminedRemotePoint(address, port));

    public async Task<Result> RunCommand(BaseCommand command)
        => await _commandManager.RunCommand(command);

    public async Task<Result<TabularData>> RunQuery(Query query)
        => await _queryManager.RunQuery(query);

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
        => await _communicationNode.SendHello(new UndeterminedRemotePoint(address, port));

    public async Task<Result> UnregisterRemotePoint(RemotePoint remotePoint)
        => await _communicationNode.SendBye(remotePoint);
}
