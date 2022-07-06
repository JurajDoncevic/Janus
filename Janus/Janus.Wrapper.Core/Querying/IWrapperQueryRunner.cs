using Janus.Commons.DataModels;

namespace Janus.Wrapper.Core.Querying;

public interface IWrapperQueryRunner<TDestinationQuery>
{
    Task<Result<TabularData>> RunQuery(TDestinationQuery query);
}