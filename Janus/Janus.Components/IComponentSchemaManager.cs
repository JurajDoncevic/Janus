using Janus.Commons.SchemaModels;
using Janus.Communication.Remotes;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Components;

public interface IComponentSchemaManager
{
    /// <summary>
    /// Gets the currently loaded schema
    /// </summary>
    /// <returns></returns>
    public Option<DataSource> GetCurrentOutputSchema();

    /// <summary>
    /// Reloads the schema from the current data sources
    /// </summary>
    /// <returns></returns>
    public Task<Result<DataSource>> ReloadOutputSchema();

}

public interface IDelegatingSchemaManager
{
    /// <summary>
    /// Gets the data source schema from a specific (registered) remote component
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public Task<Result<DataSource>> GetDataSourceSchemaFrom(RemotePoint remotePoint);

    /// <summary>
    /// Gets all schemas from registered remote points - does not change manager state
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<Result<DataSource>>> GetSchemasFromComponents();

    /// <summary>
    /// Reloads all schemas from registered remote points - changes manager state
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<Result<DataSource>>> ReloadSchemasFromComponents();
}

public interface IMediatingSchemaManager
{
    /// <summary>
    /// Generates a mediated schema from the current data sources with given mediation
    /// </summary>
    /// <param name="mediation"></param>
    /// <returns></returns>
    public Task<Result<DataSource>> GenerateMediatedSchema(DataSourceMediation mediation);
}