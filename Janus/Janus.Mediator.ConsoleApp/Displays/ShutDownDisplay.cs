using FunctionalExtensions.Base.Results;
using Janus.Mediator.Core;
using Janus.Logging;

namespace Janus.Mediator.ConsoleApp.Displays;
public class ShutDownDisplay : BaseDisplay
{
    private readonly ILogger<ShutDownDisplay>? _logger;
    public ShutDownDisplay(MediatorController mediatorController, ILogger? logger = null) : base(mediatorController)
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
