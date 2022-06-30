using Janus.Commons.DataModels;

namespace Janus.Wrapper.Core.Querying;

public interface IWrapperQueryRunner<TLocalQuery>
{
    Task<Result<TabularData>> RunQuery(TLocalQuery query);
}