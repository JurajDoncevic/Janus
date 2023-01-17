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
using Janus.Mask.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    {
        throw new NotImplementedException();
    }

    public IEnumerable<RemotePoint> GetRegisteredRemotePoints()
    {
        throw new NotImplementedException();
    }

    public Task<Result<RemotePoint>> RegisterRemotePoint(UndeterminedRemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result<RemotePoint>> RegisterRemotePoint(string address, int port)
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

    public Task<Result<RemotePoint>> SendHello(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }

    public Task<Result<RemotePoint>> SendHello(string address, int port)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UnregisterRemotePoint(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }
}
