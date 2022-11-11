using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.QueryMediationModels;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediation;
/// <summary>
/// Query model mediation operations
/// </summary>
public static class QueryModelMediation
{
    public static Result<QueryMediation> MediateQuery(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, Query query)
        => ResultExtensions.AsResult(() =>
        {
            // determine split queries - for each join set from the same data source - split a colored graph

            // split projection into separate queries

            // if conjunctive selection split selection into separate queries, else put into finalizing selection

            return Result<QueryMediation>.OnException(new NotImplementedException());
        });

    public static Result<TabularData> MediateQueryResults(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, QueryMediation queryMediation)
        => ResultExtensions.AsResult(() =>
        {

            return Result<TabularData>.OnException(new NotImplementedException());
        });
}
