using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.MediationModels;

namespace Janus.Mediation;
/// <summary>
/// Query model mediation operations
/// </summary>
public static class QueryModelMediation
{
    public static Result<Dictionary<string, Query>> DemediateQuery(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, Query mediatedQuery)
        => ResultExtensions.AsResult<Dictionary<string, Query>>(() =>
        {

            return Result<Dictionary<string, Query>>.OnException(new NotImplementedException());
        });
}
