using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediation;
/// <summary>
/// Command model mediation operations
/// </summary>
public class CommandModelMediation
{
    public static Result<BaseCommand> DemediateCommand(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, BaseCommand baseCommand)
    => Results.AsResult<BaseCommand>(() =>
    {
        return Results.OnException<BaseCommand>(new NotImplementedException());
    });
}
