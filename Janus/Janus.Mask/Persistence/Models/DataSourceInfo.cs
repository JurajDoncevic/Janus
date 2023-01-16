using Janus.Commons.SchemaModels;

namespace Janus.Mask.Persistence.Models;
public sealed class DataSourceInfo
{
    private readonly DataSource _inferredDataSource;
    private readonly DateTime _createdOn;

    public DataSource InferredDataSource => _inferredDataSource;
    public DateTime CreatedOn => _createdOn;

    public DataSourceInfo(DataSource inferredDataSource, DateTime? createdOn = null)
    {
        _inferredDataSource = inferredDataSource;
        _createdOn = createdOn ?? DateTime.Now;
    }
}
