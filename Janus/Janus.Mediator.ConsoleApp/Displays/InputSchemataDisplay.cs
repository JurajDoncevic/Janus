using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.ConsoleApp.Displays;
public class InputSchemataDisplay : BaseDisplay
{
    private readonly ILogger<InputSchemataDisplay>? _logger;
    public InputSchemataDisplay(MediatorController mediatorController, ILogger? logger) : base(mediatorController)
    {
        _logger = logger.ResolveLogger<InputSchemataDisplay>();
    }

    public override string Title => "ALL INPUT SCHEMATA";

    protected async override Task<Result> Display()
        => await ResultExtensions.AsResult(async () =>
        {
            return (await _mediatorController.GetInputSchemata())
                .Map(result => result.Pass(
                        r => Console.WriteLine(r.Data!.ToString()),
                        r => Console.WriteLine(r.Message))
                    )
                .All(result => result);
        });
}
