﻿using Janus.Commons.SchemaModels;
using Janus.Communication.Remotes;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Components;

public interface IComponentSchemaManager
{
    /// <summary>
    /// Gets the current output schema
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
    /// Gets the schema from a specific remote component
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    public Task<Result<DataSource>> GetSchemaFrom(RemotePoint remotePoint);

    /// <summary>
    /// Include a data source schema from a remote point in the loaded schemas. Data sources with same names can't be loaded at the same time
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public Task<Result<DataSource>> LoadSchema(RemotePoint remotePoint);

    /// <summary>
    /// Removes a remote point from schema inferrence
    /// </summary>
    /// <param name="remotePoint"></param>
    /// <returns></returns>
    public Result UnloadSchema(RemotePoint remotePoint);
}

public interface IMediatingSchemaManager
{
    /// <summary>
    /// Applies the mediation to the loaded schemas
    /// </summary>
    /// <param name="mediation"></param>
    /// <returns></returns>
    public Task<Result<DataSource>> MediateLoadedSchemas(DataSourceMediation mediation);

    /// <summary>
    /// Gets all schemas from all registered remote points
    /// </summary>
    /// <returns></returns>
    public Task<Dictionary<RemotePoint, Result<DataSource>>> GetSchemasFromComponents();

    /// <summary>
    /// Reloads all loaded schemas
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<Result<DataSource>>> ReloadSchemas();
}