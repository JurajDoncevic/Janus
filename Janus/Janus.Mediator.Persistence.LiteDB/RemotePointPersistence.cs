using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Communication.Remotes;
using Janus.Components.Persistence;
using Janus.Logging;
using Janus.Mediator.Persistence.LiteDB.DbModels;
using LiteDB;

namespace Janus.Mediator.Persistence.LiteDB;
public sealed class RemotePointPersistence : IRemotePointPersistence, IDisposable
{
    private readonly ILiteDatabase _database;
    private readonly ILogger<RemotePointPersistence>? _logger;

    public RemotePointPersistence(ILiteDatabase liteDatabase, ILogger? logger = null)
    {
        _database = liteDatabase;
        _logger = logger?.ResolveLogger<RemotePointPersistence>();
    }

    public RemotePointPersistence(string databaseFilePath, ILogger? logger = null)
    {
        _database = new LiteDatabase(databaseFilePath);
        _logger = logger?.ResolveLogger<RemotePointPersistence>();
    }

    public Result Delete(string nodeId)
        => Results.AsResult(() => _database.GetCollection<DbModels.RemotePointInfo>().Delete(nodeId))
            .Pass(r => _logger?.Info($"Deleted remote point with node id {nodeId} from persistence"),
                  r => _logger?.Info($"Failed to deleted remote point with node id {nodeId} from persistence"));

    public Result<RemotePoint> Get(string nodeId)
        => Results.AsResult(() =>
            _database.GetCollection<DbModels.RemotePointInfo>()
                     .FindById(nodeId)
                     .Identity()
                     .Map(dbModel => (RemotePoint)(dbModel switch
                     {
                         { RemotePointType: RemotePointTypes.MASK } => new MaskRemotePoint(dbModel.NodeId, dbModel.Address, dbModel.ListenPort),
                         { RemotePointType: RemotePointTypes.WRAPPER } => new WrapperRemotePoint(dbModel.NodeId, dbModel.Address, dbModel.ListenPort),
                         { RemotePointType: RemotePointTypes.MEDIATOR } => new MediatorRemotePoint(dbModel.NodeId, dbModel.Address, dbModel.ListenPort),
                         { RemotePointType: RemotePointTypes.UNDETERMINED } => new UndeterminedRemotePoint(dbModel.Address, dbModel.ListenPort),
                         _ => new UndeterminedRemotePoint(dbModel.Address, dbModel.ListenPort)
                     })).Data
        ).Pass(r => _logger?.Info($"Got remote point with node id {nodeId} from persistence"),
               r => _logger?.Info($"Failed to get remote point with node id {nodeId} from persistence"));

    public Result<IEnumerable<RemotePoint>> GetAll()
        => Results.AsResult(() =>
            _database.GetCollection<DbModels.RemotePointInfo>()
                     .FindAll()
                     .Map(dbModel => (RemotePoint)(dbModel switch
                     {
                         { RemotePointType: RemotePointTypes.MASK } => new MaskRemotePoint(dbModel.NodeId, dbModel.Address, dbModel.ListenPort),
                         { RemotePointType: RemotePointTypes.WRAPPER } => new WrapperRemotePoint(dbModel.NodeId, dbModel.Address, dbModel.ListenPort),
                         { RemotePointType: RemotePointTypes.MEDIATOR } => new MediatorRemotePoint(dbModel.NodeId, dbModel.Address, dbModel.ListenPort),
                         { RemotePointType: RemotePointTypes.UNDETERMINED } => new UndeterminedRemotePoint(dbModel.Address, dbModel.ListenPort),
                         _ => new UndeterminedRemotePoint(dbModel.Address, dbModel.ListenPort)
                     }))
        ).Pass(r => _logger?.Info("Got all remote points from persistence"),
               r => _logger?.Info("Failed to get all remote points from persistence"));

    public Result Insert(RemotePoint model)
        => Results.AsResult(
            () => !_database.GetCollection<DbModels.RemotePointInfo>()
                            .Insert(new DbModels.RemotePointInfo()
                            {
                                NodeId = model.NodeId,
                                Address = model.Address,
                                ListenPort = model.Port,
                                RemotePointType = model.RemotePointType
                            }).IsNull
            ).Pass(r => _logger?.Info($"Inserted remote point {model} into persistence"),
                   r => _logger?.Info($"Failed insert remote point {model} into persistence"));

    public void Dispose()
    {
        _database?.Dispose();
    }

    public bool Exists(string id)
        => Results.AsResult(() => _database.GetCollection<RemotePointInfo>().Exists(id))
                  .Match(
                    r => true,
                    r => false,
                    r => false
                    );
}
