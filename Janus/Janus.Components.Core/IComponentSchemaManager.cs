using Janus.Commons.SchemaModels;

namespace Janus.Components.Core;

public interface IComponentSchemaManager
{
    public Result<DataSource> GetCurrentSchema();
    public Result<DataSource> GetSchemaFromNode(string nodeId);
    public Result<DataSource> ReloadSchema(object transformations = null);
}
