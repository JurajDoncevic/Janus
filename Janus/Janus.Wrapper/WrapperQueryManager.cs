﻿using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Components;
using Janus.Logging;
using Janus.Wrapper.LocalQuerying;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper;
/// <summary>
/// Query manager for a wrapper component
/// </summary>
/// <typeparam name="TLocalQuery"></typeparam>
/// <typeparam name="TSelection"></typeparam>
/// <typeparam name="TJoining"></typeparam>
/// <typeparam name="TProjection"></typeparam>
/// <typeparam name="TLocalData"></typeparam>
public abstract class WrapperQueryManager<TLocalQuery, TSelection, TJoining, TProjection, TLocalData>
    : IComponentQueryManager
    where TLocalQuery : LocalQuery<TSelection, TJoining, TProjection>
{
    private readonly IWrapperQueryTranslator<TLocalQuery, TSelection, TJoining, TProjection> _queryTranslator;
    private readonly IWrapperDataTranslator<TLocalData> _dataTranslator;
    private readonly IQueryExecutor<TSelection, TJoining, TProjection, TLocalData, TLocalQuery> _queryExecutor;
    private readonly ILogger<WrapperQueryManager<TLocalQuery, TSelection, TJoining, TProjection, TLocalData>>? _logger;
    public WrapperQueryManager(
        IWrapperQueryTranslator<TLocalQuery, TSelection, TJoining, TProjection> queryTranslator,
        IWrapperDataTranslator<TLocalData> dataTranslator,
        IQueryExecutor<TSelection, TJoining, TProjection, TLocalData, TLocalQuery> queryExecutor,
        ILogger? logger = null)
    {
        _queryTranslator = queryTranslator;
        _dataTranslator = dataTranslator;
        _queryExecutor = queryExecutor;
        _logger = logger?.ResolveLogger<WrapperQueryManager<TLocalQuery, TSelection, TJoining, TProjection, TLocalData>>();
    }

    public async Task<Result<TabularData>> RunQuery(Query query)
        => (await Task.FromResult(_queryTranslator.Translate(query))
            .Bind(_queryExecutor.ExecuteQuery))
            .Bind(_dataTranslator.Translate)
            .Pass(r => _logger?.Info($"Command {query.Name} ran successfully."),
                  r => _logger?.Info($"Failed command {query.Name} run with message: {r.Message}"));

}
