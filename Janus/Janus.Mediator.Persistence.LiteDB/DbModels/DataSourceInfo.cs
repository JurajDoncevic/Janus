using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.Persistence.LiteDB.DbModels;
internal sealed class DataSourceInfo
{
    internal DataSourceInfo(string mediatedDataSourceVersion, string mediatedDataSourceJson, string mediationScript, Dictionary<RemotePointInfo, string> loadedDataSources, DateTime? persistedOn = null)
    {
        Version = mediatedDataSourceVersion;
        MediatedDataSourceJson = mediatedDataSourceJson;
        MediationScript = mediationScript;
        LoadedDataSourcesJsons = loadedDataSources.ToDictionary(kv => kv.Key.NodeId, kv => new RemotePointDataSource { RemotePoint = kv.Key, DataSourceJson = kv.Value });
        PersistedOn = persistedOn ?? DateTime.Now;
    }

    public DataSourceInfo() { }

    [BsonId(false)]
    public string Version { get; init; }
    public string MediatedDataSourceJson { get; init; }
    public string MediationScript { get; private set; }
    public Dictionary<string, RemotePointDataSource> LoadedDataSourcesJsons { get; init; }
    public DateTime PersistedOn { get; init; }
}

internal sealed class RemotePointDataSource
{
    public RemotePointInfo RemotePoint { get; init; }
    public string DataSourceJson { get; set; } = string.Empty;
}
