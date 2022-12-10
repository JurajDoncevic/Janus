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
            return (await _mediatorController.GetAvailableSchemas())
                .Map(result => result.Pass(
                        r => Console.WriteLine(r.Data!.ToString()),
                        r => Console.WriteLine(r.Message))
                    )
                .All(result => result);
        });
}
