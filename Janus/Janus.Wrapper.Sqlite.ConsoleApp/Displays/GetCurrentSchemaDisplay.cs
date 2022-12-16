using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Logging;

namespace Janus.Wrapper.Sqlite.ConsoleApp.Displays;
public class GetCurrentSchemaDisplay : BaseDisplay
{
    private readonly ILogger<GetCurrentSchemaDisplay>? _logger;

    public GetCurrentSchemaDisplay(SqliteWrapperManager wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<GetCurrentSchemaDisplay>();
    }

    public override string Title => "CURRENT SCHEMA";

    protected override async Task<Result> Display()
        => _wrapperController.GetCurrentSchema()
            .Pass(result =>
            {
                if (result)
                {
                    Console.WriteLine(result.Value?.ToString());
                }
                else
                {
                    Console.WriteLine("No schema generated");
                }
            })
            .Match(r => Results.OnSuccess(), () => Results.OnFailure());


}
