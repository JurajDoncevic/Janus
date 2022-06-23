using Janus.Commons.SchemaModels;

namespace Janus.Components;

public interface IComponentSchemaManager
{
    public Task<Result<DataSource>> GetCurrentSchema();
    public Task<Result<DataSource>> GetSchemaFromNode(string nodeId);
    public Task<Result<DataSource>> ReloadSchema(object transformations = null);
}
