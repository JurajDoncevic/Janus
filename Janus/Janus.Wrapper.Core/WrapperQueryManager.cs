using FunctionalExtensions.Base.Results;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Components;
using Janus.Wrapper.Core.Translation;
using Janus.Wrapper.Core.LocalQuerying;

namespace Janus.Wrapper.Core;
public sealed class WrapperQueryManager<TSelection, TJoining, TProjection, TLocalData> : IComponentQueryManager
{
    private readonly ILocalQueryTranslator<LocalQuery<TSelection, TJoining, TProjection>, TSelection, TJoining, TProjection> _queryTranslator;
    private readonly ILocalDataTranslator<TLocalData> _dataTranslator;
    private readonly IQueryExecutor<TSelection, TJoining, TProjection, TLocalData> _queryExecutor;

    public WrapperQueryManager(
        ILocalQueryTranslator<LocalQuery<TSelection, TJoining, TProjection>, TSelection, TJoining, TProjection> queryTranslator,
        ILocalDataTranslator<TLocalData> dataTranslator,
        IQueryExecutor<TSelection, TJoining, TProjection, TLocalData> queryExecutor)
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
