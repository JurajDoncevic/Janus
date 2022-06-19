using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Remotes;
using Janus.Components.Core;

namespace Janus.Mediator.Core;
public class MediatorController : IComponentController
{
    private readonly MediatorQueryManager _queryManager;
    private readonly MediatorCommandManager _commandManager;
    private readonly MediatorSchemaManager _schemaManager;

    public MediatorController(MediatorQueryManager queryManager, MediatorCommandManager commandManager, MediatorSchemaManager schemaManager)
    {
        _queryManager = queryManager;
        _commandManager = commandManager;
        _schemaManager = schemaManager;
    }

    public List<RemotePoint> GetRegisteredRemotePoints()
    {
        throw new NotImplementedException();
    }

    public Task<Result<DataSource>> GetSchema()
    {
        throw new NotImplementedException();
    }

    public Result RegisterComponent(string address, int port)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RunCommand(BaseCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<Result<TabularData>> RunQuery(Query query)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveRegisteredRemotePoints(string filePath)
    {
        throw new NotImplementedException();
    }

    public Result<RemotePoint> SendHello(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Result<RemotePoint> SendHello(string address, int port)
    {
        throw new NotImplementedException();
    }

    public Result UnregisterComponent(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }
}
