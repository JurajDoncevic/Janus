using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;

namespace Janus.Components;

public interface IComponentQueryManager
{
    /// <summary>
    /// Executes a query on this component's schema
    /// </summary>
    /// <param name="query">Query to execute</param>
    /// <returns></returns>
    public Task<Result<TabularData>> RunQuery(Query query);
}

public interface IDelegatingQueryManager
{
    /// <summary>
    /// Executes a query on a remote component
    /// </summary>
    /// <param name="query"></param>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    public Task<Result<TabularData>> RunQueryOnComponent(Query query, string nodeId);
}