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
        => Results.AsResult(() =>
        {
            // determine split queries - for each join set from the same data source - split a colored graph

            // split projection into separate queries - keep the joining attributes

            // if conjunctive selection split selection into separate queries, else put into finalizing selection

            return Results.OnException<QueryMediation>(new NotImplementedException());
        });

    public static Result<TabularData> MediateQueryResults(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, QueryMediation queryMediation)
        => Results.AsResult(() =>
        {

            return Results.OnException<TabularData>(new NotImplementedException());
        });
}
