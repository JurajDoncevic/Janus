using Janus.Commons.SchemaModels;

namespace Janus.Components;

public interface IComponentSchemaManager
{
    /// <summary>
    /// Gets the currently loaded schema
    /// </summary>
    /// <returns></returns>
    public Task<Result<DataSource>> GetCurrentSchema();
    /// <summary>
    /// Reloads the schema from the current data sources
    /// </summary>
    /// <param name="transformations"></param>
    /// <returns></returns>
    public Task<Result<DataSource>> ReloadSchema(object? transformations = null);

}

public interface IDelegatingSchemaManager
{
    /// <summary>
    /// Gets the schema from a specific remote component
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    public Task<Result<DataSource>> GetSchemaFromComponent(string nodeId);
}