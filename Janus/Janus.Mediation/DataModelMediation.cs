using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediation;
/// <summary>
/// Data model mediation operations
/// </summary>
public class DataModelMediation
{
    public static Result<TabularData> MediateData(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, TabularData tabularData)
        => Results.AsResult<TabularData>(() =>
        {
            return Results.OnException<TabularData>(new NotImplementedException());
        });

    public static Result<TabularData> DemediateData(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, TabularData tabularData)
        => Results.AsResult<TabularData>(() =>
        {
            return Results.OnException<TabularData>(new NotImplementedException());
        });
}
