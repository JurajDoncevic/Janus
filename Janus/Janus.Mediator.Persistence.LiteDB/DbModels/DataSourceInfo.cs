using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.Persistence.LiteDB.DbModels;
internal sealed class DataSourceInfo
{
    internal DataSourceInfo(string mediatedDataSourceVersion, string mediatedDataSourceJson, string mediationScript, IEnumerable<string> loadedDataSources, DateTime? persistedOn = null)
    {
        Version = mediatedDataSourceVersion;
        MediatedDataSourceJson = mediatedDataSourceJson;
        MediationScript = mediationScript;
        LoadedDataSourcesJsons = loadedDataSources;
        PersistedOn = persistedOn ?? DateTime.Now;
    }

    [BsonId(false)]
    public string Version { get; init; }
    public string MediatedDataSourceJson { get; init; }
    public string MediationScript { get; private set; }
    public IEnumerable<string> LoadedDataSourcesJsons { get; init; }
    public DateTime PersistedOn { get; init; }
}
