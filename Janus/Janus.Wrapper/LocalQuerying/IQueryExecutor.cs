using FunctionalExtensions.Base.Results;

namespace Janus.Wrapper.LocalQuerying;

public interface IQueryExecutor<TSelection, TJoining, TProjection, TLocalData, TLocalQuery> where TLocalQuery 
    : LocalQuery<TSelection, TJoining, TProjection>
{
    public Task<Result<TLocalData>> ExecuteQuery(TLocalQuery localQuery);
}