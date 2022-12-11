using Janus.Commons.SchemaModels;
using Janus.Communication.Remotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.Persistence.Models;
public sealed class DataSourceInfo
{
    private readonly DataSource _mediatedDataSource;
    private readonly string _mediationScript;
    private readonly Dictionary<RemotePoint, DataSource> _loadedDataSources;
    private readonly DateTime _createdOn;

    public DataSource MediatedDataSource => _mediatedDataSource;
    public string MediationScript => _mediationScript;
    public IReadOnlyDictionary<RemotePoint, DataSource> LoadedDataSources => _loadedDataSources;
    public DateTime CreatedOn => _createdOn;

    public DataSourceInfo(DataSource mediatedDataSource, string mediationScript, Dictionary<RemotePoint, DataSource> loadedDataSources, DateTime? createdOn = null)
    {
        _mediatedDataSource = mediatedDataSource;
        _mediationScript = mediationScript;
        _loadedDataSources = loadedDataSources;
        _createdOn = createdOn ?? DateTime.Now;
    }
}
