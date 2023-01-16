using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Logging;
using Janus.Mask.Persistence.LiteDB.DbModels;
using LiteDB;
using Janus.Serialization.Json;
using Janus.Serialization.Json.SchemaModels;

namespace Janus.Mask.Persistence.LiteDB;
public sealed class DataSourceInfoPersistence : IDataSourceInfoPersistence, IDisposable
{
    private readonly ILiteDatabase _database;
    private readonly DataSourceSerializer _dataSourceSerializer;
    private readonly ILogger<DataSourceInfoPersistence>? _logger;

    public DataSourceInfoPersistence(ILiteDatabase liteDatabase, JsonSerializationProvider jsonSerializationProvider, ILogger? logger = null)
    {
        if (jsonSerializationProvider is null)
        {
            throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        _database = liteDatabase ?? throw new ArgumentNullException(nameof(liteDatabase));
        _dataSourceSerializer = (DataSourceSerializer)jsonSerializationProvider.DataSourceSerializer;
        _logger = logger?.ResolveLogger<DataSourceInfoPersistence>();
    }

    public DataSourceInfoPersistence(string databaseFilePath, JsonSerializationProvider jsonSerializationProvider, ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(databaseFilePath))
        {
            throw new ArgumentException($"'{nameof(databaseFilePath)}' cannot be null or empty.", nameof(databaseFilePath));
        }

        if (jsonSerializationProvider is null)
        {
            throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        _database = new LiteDatabase(databaseFilePath);
        _dataSourceSerializer = (DataSourceSerializer)jsonSerializationProvider.DataSourceSerializer;
        _logger = logger?.ResolveLogger<DataSourceInfoPersistence>();
    }

    public Result Delete(string version)
        => Results.AsResult(() => _database.GetCollection<DbModels.DataSourceInfo>().Delete(version))
                  .Pass(r => _logger?.Info($"Deleted data source info for version {version} from persistence"),
                        r => _logger?.Info($"Failed to delete data source info for version {version} from persistence"));

    public Result<Models.DataSourceInfo> Get(string version)
        => Results.AsResult<Models.DataSourceInfo>(() =>
        {
            var dbModel = _database.GetCollection<DbModels.DataSourceInfo>()
                           .FindById(version);

            if (dbModel is null)
            {
                return Results.OnFailure<Models.DataSourceInfo>($"No data source with version {version}");
            }

            var inferredDataSourceDeserialization = _dataSourceSerializer.Deserialize(dbModel.InferredDataSourceJson);
            if (!inferredDataSourceDeserialization)
            {
                return inferredDataSourceDeserialization.Map(_ => (Models.DataSourceInfo)null!);
            }

            return new Models.DataSourceInfo(inferredDataSourceDeserialization.Data, dbModel.PersistedOn);
        }).Pass(r => _logger?.Info($"Got data source info for version {version} from persistence"),
                r => _logger?.Info($"Failed to get data source info for version {version} from persistence"));

    public Result<IEnumerable<Models.DataSourceInfo>> GetAll()
        => Results.AsResult<IEnumerable<Models.DataSourceInfo>>(() =>
        {
            var dbModels = _database.GetCollection<DbModels.DataSourceInfo>()
                           .FindAll();

            if (dbModels is null)
            {
                return Results.OnFailure<IEnumerable<Models.DataSourceInfo>>($"Couldn't load data sources from persistence");
            }

            var dataSourceInfos = Enumerable.Empty<Models.DataSourceInfo>();
            foreach (var dbModel in dbModels)
            {
                var inferredDataSourceDeserialization = _dataSourceSerializer.Deserialize(dbModel.InferredDataSourceJson);
                if (!inferredDataSourceDeserialization)
                {
                    return inferredDataSourceDeserialization.Map(_ => Enumerable.Empty<Models.DataSourceInfo>());
                }

                dataSourceInfos = dataSourceInfos = dataSourceInfos.Append(new Models.DataSourceInfo(inferredDataSourceDeserialization.Data, dbModel.PersistedOn));
            }


            return Results.OnSuccess(dataSourceInfos);

        }).Pass(r => _logger?.Info($"Got all data source info for version from persistence"),
                r => _logger?.Info($"Failed to get all data source info from persistence"));

    public Result<Models.DataSourceInfo> GetLatest()
        => Results.AsResult<Models.DataSourceInfo>(() =>
        {
            var dbModel = _database.GetCollection<DbModels.DataSourceInfo>()
                                   .FindOne(Query.All("PersistedOn", Query.Descending));

            if (dbModel is null)
            {
                return Results.OnFailure<Models.DataSourceInfo>($"No data source info in persistence");
            }

            var inferredDataSourceDeserialization = _dataSourceSerializer.Deserialize(dbModel.InferredDataSourceJson);
            if (!inferredDataSourceDeserialization)
            {
                return inferredDataSourceDeserialization.Map(_ => (Models.DataSourceInfo)null!);
            }

            return new Models.DataSourceInfo(inferredDataSourceDeserialization.Data, dbModel.PersistedOn);
        }).Pass(r => _logger?.Info($"Got latest data source info persisted on {r.Data.CreatedOn} with version {r.Data.InferredDataSource.Version} from persistence"),
                r => _logger?.Info($"Failed to get latest data source info from persistence"));

    public Result Insert(Models.DataSourceInfo model)
        => Results.AsResult(() =>
        {
            var serialization =
                _dataSourceSerializer.Serialize(model.InferredDataSource);

            if (!serialization)
            {
                return Results.OnFailure($"Failed to serialize inferred data source: {serialization.Message}");
            }

            var dbModel = new DbModels.DataSourceInfo(model.InferredDataSource.Version, serialization.Data, model.CreatedOn);

            return Results.AsResult(
                () => !_database.GetCollection<DbModels.DataSourceInfo>()
                                .Insert(dbModel).IsNull
                     );

        }).Pass(r => _logger?.Info($"Inserted data source info with version {model.InferredDataSource.Version} into persistence"),
                r => _logger?.Info($"Failed to insert data source info into persistence: {r.Message}"));

    public bool Exists(string id)
        => Results.AsResult(() => _database.GetCollection<DataSourceInfo>().Exists(id))
                  .Match(
                    r => true,
                    r => false,
                    r => false
                    );

    public void Dispose()
    {
        _database?.Dispose();
    }
}
