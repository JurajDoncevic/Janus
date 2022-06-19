using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;

namespace Janus.Components.Core;

public interface IComponentQueryManager
{
    public TabularData ExecuteQueryOnNode(Query query, string nodeId);
    public TabularData ExecuteQueryGlobally(Query query);
}
