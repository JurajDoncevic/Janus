using Janus.Commons.SchemaModels;
using Janus.Communication.Remotes;

namespace Janus.Components;

public interface IComponentSchemaManager
{
    /// <summary>
    /// Gets the currently loaded schema
    /// </summary>
    /// <returns></returns>
    public Task<Result<DataSource>> GetCurrentOutputSchema();
    /// <summary>
    /// Reloads the schema from the current data sources
    /// </summary>
    /// <returns></returns>
    public Task<Result<DataSource>> ReloadOutputSchema();

}

public interface IDelegatingSchemaManager
{
    /// <summary>
    /// Gets the schema from a specific remote component
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    public Task<Result<DataSource>> GetInputSchemaFromComponent(string nodeId);

    /// <summary>
    /// Gets all schemata from remote points assigned for schema inferrence
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<Result<DataSource>>> GetInputSchemataFromComponents();

    /// <summary>
    /// Adds a remote point to schema inferrence
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public Result AddRemotePointToSchemaInferrence(RemotePoint remotePoint);

    /// <summary>
    /// Removes a remote point from schema inferrence
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public Result RemoveRemotePointFromSchemaInferrence(RemotePoint remotePoint);
}

public interface ITransformingSchemaManager
{
    /// <summary>
    /// Reloads the schema from the current data sources with transformations applied
    /// </summary>
    /// <param name="transformations"></param>
    /// <returns></returns>
    public Task<Result<DataSource>> ReloadSchema(object? transformations = null);
}