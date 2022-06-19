using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;

namespace Janus.Components.Core;

public interface IComponentQueryManager
{
    public Task<Result<TabularData>> ExecuteQueryOnNode(Query query, string nodeId);
    public Task<Result<TabularData>> ExecuteQueryGlobally(Query query);
}
