using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Logging;

namespace Janus.Mediator.ConsoleApp.Displays;
public class InputSchemataDisplay : BaseDisplay
{
    private readonly ILogger<InputSchemataDisplay>? _logger;
    public InputSchemataDisplay(MediatorManager mediatorManager, ILogger? logger) : base(mediatorManager)
    {
        _logger = logger.ResolveLogger<InputSchemataDisplay>();
    }

    public override string Title => "ALL INPUT SCHEMATA";

    protected async override Task<Result> Display()
        => await Results.AsResult(async () =>
        {
            return Results.OnException(new NotImplementedException());
        });
}
