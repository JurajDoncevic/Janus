using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Logging;
using Janus.Serialization.Json;
using Janus.Serialization.Json.SchemaModels;
using LiteDB;

namespace Janus.Mediator.Persistence.LiteDB;
public sealed class DataSourcePersistence : IDataSourceInfoPersistence
{
    private readonly ILiteDatabase _database;
    private readonly DataSourceSerializer _dataSourceSerializer;
    private readonly ILogger<DataSourcePersistence>? _logger;

    public DataSourcePersistence(ILiteDatabase liteDatabase, JsonSerializationProvider jsonSerializationProvider, ILogger? logger = null)
    {
        if (jsonSerializationProvider is null)
        {
            throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        _database = liteDatabase ?? throw new ArgumentNullException(nameof(liteDatabase));
        _dataSourceSerializer = (DataSourceSerializer)jsonSerializationProvider.DataSourceSerializer;
        _logger = logger?.ResolveLogger<DataSourcePersistence>();
    }

    public DataSourcePersistence(string databaseFilePath, JsonSerializationProvider jsonSerializationProvider, ILogger? logger = null)
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
        _logger = logger?.ResolveLogger<DataSourcePersistence>();
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

            var mediatedDataSourceDeserialization = _dataSourceSerializer.Deserialize(dbModel.MediatedDataSourceJson);
            if (!mediatedDataSourceDeserialization)
            {
                return mediatedDataSourceDeserialization.Map(_ => (Models.DataSourceInfo)null!);
            }

            var loadedDataSourcesDeserializations = 
                dbModel.LoadedDataSourcesJsons.Map(json => _dataSourceSerializer.Deserialize(json))
                       .Fold(Results.OnSuccess(Enumerable.Empty<Commons.SchemaModels.DataSource>()),
                        (deserialization, results) => deserialization.Bind(dataSource => results.Map(_ => _.Append(dataSource)))
                       );

            if (!loadedDataSourcesDeserializations)
            {
                return mediatedDataSourceDeserialization.Map(_ => (Models.DataSourceInfo)null!);
            }

            return new Models.DataSourceInfo(mediatedDataSourceDeserialization.Data, dbModel.MediationScript, loadedDataSourcesDeserializations.Data);
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
                var mediatedDataSourceDeserialization = _dataSourceSerializer.Deserialize(dbModel.MediatedDataSourceJson);
                if (!mediatedDataSourceDeserialization)
                {
                    return mediatedDataSourceDeserialization.Map(_ => Enumerable.Empty<Models.DataSourceInfo>());
                }

                var loadedDataSourcesDeserializations =
                    dbModel.LoadedDataSourcesJsons.Map(json => _dataSourceSerializer.Deserialize(json))
                           .Fold(Results.OnSuccess(Enumerable.Empty<Commons.SchemaModels.DataSource>()),
                            (deserialization, results) => deserialization.Bind(dataSource => results.Map(_ => _.Append(dataSource)))
                           );

                if (!loadedDataSourcesDeserializations)
                {
                    return mediatedDataSourceDeserialization.Map(_ => Enumerable.Empty<Models.DataSourceInfo>());
                }

                dataSourceInfos = dataSourceInfos = dataSourceInfos.Append(new Models.DataSourceInfo(mediatedDataSourceDeserialization.Data, dbModel.MediationScript, loadedDataSourcesDeserializations.Data, dbModel.PersistedOn));
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

            var mediatedDataSourceDeserialization = _dataSourceSerializer.Deserialize(dbModel.MediatedDataSourceJson);
            if (!mediatedDataSourceDeserialization)
            {
                return mediatedDataSourceDeserialization.Map(_ => (Models.DataSourceInfo)null!);
            }

            var loadedDataSourcesDeserializations =
                dbModel.LoadedDataSourcesJsons.Map(json => _dataSourceSerializer.Deserialize(json))
                       .Fold(Results.OnSuccess(Enumerable.Empty<Commons.SchemaModels.DataSource>()),
                        (deserialization, results) => deserialization.Bind(dataSource => results.Map(_ => _.Append(dataSource)))
                       );

            if (!loadedDataSourcesDeserializations)
            {
                return mediatedDataSourceDeserialization.Map(_ => (Models.DataSourceInfo)null!);
            }

            return new Models.DataSourceInfo(mediatedDataSourceDeserialization.Data, dbModel.MediationScript, loadedDataSourcesDeserializations.Data);
        }).Pass(r => _logger?.Info($"Got latest data source info persisted on {r.Data.CreatedOn } with version {r.Data.MediatedDataSource.Version} from persistence"),
                r => _logger?.Info($"Failed to get latest data source info from persistence"));

    public Result Insert(Models.DataSourceInfo model)
        => Results.AsResult(() =>
        {
            var serializations =
                _dataSourceSerializer.Serialize(model.MediatedDataSource)
                .Bind(_ =>
                {
                    return model.LoadedDataSources
                                .Map(_dataSourceSerializer.Serialize)
                                .Fold(Results.OnSuccess(Enumerable.Empty<string>()), (serialization, resultJsons) => serialization.Bind(s => resultJsons.Map(r => r.Append(s))))
                                .Map(jsons => (mediatedDataSourceJson: _, loadedDataSourcesJsons: jsons));
                });

            if (!serializations)
            {
                return Results.OnFailure($"Failed to serialize mediated data source: {serializations.Message}");
            }

            var dbModel = new DbModels.DataSourceInfo(model.MediatedDataSource.Version, serializations.Data.mediatedDataSourceJson, model.MediationScript, serializations.Data.loadedDataSourcesJsons);

            return Results.AsResult(
                () => !_database.GetCollection<DbModels.DataSourceInfo>()
                                .Insert(dbModel).IsNull
                     );

        }).Pass(r => _logger?.Info($"Inserted data source info with version {model.MediatedDataSource.Version} into persistence"),
                r => _logger?.Info($"Failed to insert data source info into persistence: {r.Message}"));
}
