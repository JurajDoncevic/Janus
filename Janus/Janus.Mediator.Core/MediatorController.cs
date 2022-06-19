using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components.Core;

namespace Janus.Mediator.Core;
public class MediatorController : IComponentController
{
    private readonly MediatorQueryManager _queryManager;
    private readonly MediatorCommandManager _commandManager;
    private readonly MediatorSchemaManager _schemaManager;
    private readonly MediatorCommunicationNode _communicationNode;

    public MediatorController(MediatorCommunicationNode communicationNode, MediatorQueryManager queryManager, MediatorCommandManager commandManager, MediatorSchemaManager schemaManager)
    {
        _communicationNode = communicationNode;
        _queryManager = queryManager;
        _commandManager = commandManager;
        _schemaManager = schemaManager;
    }

    public List<RemotePoint> GetRegisteredRemotePoints()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<DataSource>> GetSchema()
    {
        throw new NotImplementedException();
    }

    public async Task<Result> RegisterComponent(string address, int port)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> RunCommand(BaseCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TabularData>> RunQuery(Query query)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> SaveRegisteredRemotePoints(string filePath)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<RemotePoint>> SendHello(string address, int port)
        => await _communicationNode.SendHello(new MediatorRemotePoint(address, port));

    public async Task<Result> UnregisterComponent(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

}
