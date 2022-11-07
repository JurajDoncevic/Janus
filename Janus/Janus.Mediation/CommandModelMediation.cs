using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Mediation.MediationModels;

namespace Janus.Mediation;
/// <summary>
/// Command model mediation operations
/// </summary>
public class CommandModelMediation
{
    public static Result<BaseCommand> DemediateCommand(DataSource mediatedDataSource, DataSourceMediation dataSourceMediation, BaseCommand baseCommand)
    => ResultExtensions.AsResult<BaseCommand>(() =>
    {
        return Result<BaseCommand>.OnException(new NotImplementedException());
    });
}
