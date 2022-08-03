using FunctionalExtensions.Base.Results;

namespace Janus.Wrapper.Core.LocalQuerying;

public interface IQueryExecutor<TSelection, TJoining, TProjection, TLocalData>
{
    public Task<Result<TLocalData>> ExecuteQuery(LocalQuery<TSelection, TJoining, TProjection> localQuery);
}