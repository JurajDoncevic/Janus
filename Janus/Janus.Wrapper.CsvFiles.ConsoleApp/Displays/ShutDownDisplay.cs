using Janus.Wrapper.Core;
using Janus.Utils.Logging;

namespace Janus.Wrapper.CsvFiles.ConsoleApp.Displays;
public class ShutDownDisplay : BaseDisplay
{
    private readonly ILogger<ShutDownDisplay>? _logger;
    public ShutDownDisplay(WrapperController wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<ShutDownDisplay>();
    }

    protected override void PreDisplay()
    {

    }

    protected override void PostDisplay()
    {
        
    }

    public override string Title => "SHUTDOWN";

    protected async override Task<Result> Display()
    {
        _logger?.Info("Exiting application and shutting down component");
        Environment.Exit(0);
        return await Task.FromResult(Result.OnSuccess("Exiting application"));
    }
}
