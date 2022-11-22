using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Logging;

namespace Janus.Wrapper.Sqlite.ConsoleApp.Displays;
public class GetCurrentSchemaDisplay : BaseDisplay
{
    private readonly ILogger<GetCurrentSchemaDisplay>? _logger;

    public GetCurrentSchemaDisplay(SqliteWrapperController wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<GetCurrentSchemaDisplay>();
    }

    public override string Title => "CURRENT SCHEMA";

    protected override async Task<Result> Display()
        => (await _wrapperController.GetSchema())
            .Pass(result => Console.WriteLine(result.Data?.ToString()),
                  result => Console.WriteLine(result.Message))
            .Bind(r => Results.OnSuccess());


}
