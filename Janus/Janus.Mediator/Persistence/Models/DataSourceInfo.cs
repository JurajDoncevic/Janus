using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.Persistence.Models;
public class DataSourceInfo
{
    private readonly DataSource _mediatedDataSource;
    private readonly string _mediationScript;
    private readonly IEnumerable<DataSource> _loadedDataSources;
    private readonly DateTime _createdOn;

    public DataSource MediatedDataSource => _mediatedDataSource;
    public string MediationScript => _mediationScript;
    public IReadOnlyList<DataSource> LoadedDataSources => _loadedDataSources.ToList();
    public DateTime CreatedOn => _createdOn;

    public DataSourceInfo(DataSource mediatedDataSource, string mediationScript, IEnumerable<DataSource> loadedDataSources, DateTime? createdOn = null)
    {
        _mediatedDataSource = mediatedDataSource;
        _mediationScript = mediationScript;
        _loadedDataSources = loadedDataSources;
        _createdOn = createdOn ?? DateTime.Now;
    }
}
