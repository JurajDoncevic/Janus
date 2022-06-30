using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Components;
using Janus.Components.Translation;
using Janus.Wrapper.Core.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core;
public class WrapperQueryManager<TLocalQuery, TSelectionDestination, TJoiningDestination, TProjectionDestination> : IComponentQueryManager
{
    private readonly IWrapperQueryRunner<TLocalQuery> _queryRunner;
    private readonly IQueryTranslator<Query, Selection, Joining, Projection, TLocalQuery, TSelectionDestination, TJoiningDestination, TProjectionDestination> _queryTranslator;

    public WrapperQueryManager(IWrapperQueryRunner<TLocalQuery> queryRunner, IQueryTranslator<Query, Selection, Joining, Projection, TLocalQuery, TSelectionDestination, TJoiningDestination, TProjectionDestination> queryTranslator)
    {
        _queryTranslator = queryTranslator;
        _queryRunner = queryRunner;
    }

    public async Task<Result<TabularData>> RunQuery(Query query)
    {
        var translatedQuery = _queryTranslator.Translate(query);
        var queryResult = await _queryRunner.RunQuery(translatedQuery)
                            .Bind(async data => ReconfigureReferences(data));
        return queryResult;
    }

    private Result<TabularData> ReconfigureReferences(TabularData data)
        => ResultExtensions.AsResult(() =>
        {
            var builder =
                TabularDataBuilder.InitTabularData(data.AttributeDataTypes.ToDictionary(_ => _.Key.Replace("/", "."), _ => _.Value));
            foreach (var row in data.RowData)
            {
                builder.AddRow(conf => conf.WithRowData(row.AttributeValues.ToDictionary(_ => _.Key.Replace("/", "."), _ => _.Value)));
            }

            return builder.Build();
        });
}
