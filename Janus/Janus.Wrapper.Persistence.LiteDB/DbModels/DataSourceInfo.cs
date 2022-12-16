using LiteDB;

namespace Janus.Wrapper.Persistence.LiteDB.DbModels;
internal sealed class DataSourceInfo
{
    internal DataSourceInfo(string inferredDataSourceVersion, string inferredDataSourceJson, DateTime? persistedOn = null)
    {
        Version = inferredDataSourceVersion;
        InferredDataSourceJson = inferredDataSourceJson;
        PersistedOn = persistedOn ?? DateTime.Now;
    }

    [BsonId(false)]
    public string Version { get; init; }
    public string InferredDataSourceJson { get; init; }
    public DateTime PersistedOn { get; init; }
}
