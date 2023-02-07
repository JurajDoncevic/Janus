using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Communication.Remotes;

namespace Janus.Components;

/// <summary>
/// Query manager that runs queries on its own schema
/// </summary>
public interface IComponentQueryManager
{
    /// <summary>
    /// Runs a query on this component's data source. Masks and mediators delegate, wrappers execute the query
    /// </summary>
    /// <param name="query">Query to execute</param>
    /// <returns></returns>
    public Task<Result<TabularData>> RunQuery(Query query);
}

/// <summary>
/// Query manager that can delegate query runs to other components
/// </summary>
public interface IDelegatingQueryManager
{
    /// <summary>
    /// Runs a query on a remote component
    /// </summary>
    /// <param name="query">Localized query</param>
    /// <param name="remotePoint">Remote point</param>
    /// <returns></returns>
    public Task<Result<TabularData>> RunQueryOn(Query query, RemotePoint remotePoint);
}