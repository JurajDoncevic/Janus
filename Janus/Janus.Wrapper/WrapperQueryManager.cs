using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Components;
using Janus.Wrapper.LocalQuerying;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper;
public class WrapperQueryManager<TLocalQuery, TSelection, TJoining, TProjection, TLocalData>
    : IExecutingQueryManager
    where TLocalQuery : LocalQuery<TSelection, TJoining, TProjection>
{
    private readonly ILocalQueryTranslator<TLocalQuery, TSelection, TJoining, TProjection> _queryTranslator;
    private readonly ILocalDataTranslator<TLocalData> _dataTranslator;
    private readonly IQueryExecutor<TSelection, TJoining, TProjection, TLocalData, TLocalQuery> _queryExecutor;

    public WrapperQueryManager(
        ILocalQueryTranslator<TLocalQuery, TSelection, TJoining, TProjection> queryTranslator,
        ILocalDataTranslator<TLocalData> dataTranslator,
        IQueryExecutor<TSelection, TJoining, TProjection, TLocalData, TLocalQuery> queryExecutor)
    {
        _queryTranslator = queryTranslator;
        _dataTranslator = dataTranslator;
        _queryExecutor = queryExecutor;
    }

    public async Task<Result<TabularData>> RunQuery(Query query)
        => (await Task.FromResult(_queryTranslator.Translate(query))
            .Bind(_queryExecutor.ExecuteQuery))
            .Bind(_dataTranslator.TranslateToTabularData);

}
