using FunctionalExtensions.Base.Results;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Components;
using Janus.Wrapper.Translation;
using Janus.Wrapper.LocalQuerying;

namespace Janus.Wrapper;
public sealed class WrapperQueryManager<TSelection, TJoining, TProjection, TLocalData, TLocalQuery> 
    : IComponentQueryManager
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
