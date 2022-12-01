using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.CommandMediationModels;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediation;
/// <summary>
/// Command model mediation operations
/// </summary>
public class CommandModelMediation
{
    public static Result<DeleteCommandMediation> MediateCommand(DeleteCommand deleteOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation)
        => Results.AsResult(() =>
        {
            // localize ids...

            //... of initial tableau

            //... of selection expression

            return Results.OnException<DeleteCommandMediation>(new NotImplementedException());
        });

    public static Result<UpdateCommandMediation> MediateCommand(UpdateCommand updateOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, BaseCommand baseCommand)
        => Results.AsResult(() =>
        {
            // localize ids...

            //... of initial tableau

            //... of selection expression

            //... of value updates

            return Results.OnException<UpdateCommandMediation>(new NotImplementedException());
        });

    public static Result<InsertCommandMediation> MediateCommand(InsertCommand insertOnMediatedDataSource, DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, BaseCommand baseCommand)
        => Results.AsResult(() =>
        {
            // localize ids...

            //... of initial tableau

            //... of tabular data

            return Results.OnException<InsertCommandMediation>(new NotImplementedException());
        });
}
