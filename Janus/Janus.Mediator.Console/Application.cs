using Janus.Mediator.Core;
using Janus.Utils.Logging;

namespace Janus.Mediator.Console;
public class Application
{
    private readonly MediatorController _mediatorController;
    private readonly MediatorOptions _mediatorOptions;
    private readonly ApplicationOptions _applicationOptions;
    private readonly ILogger<Application>? _logger;
    public Application(MediatorController mediatorController, MediatorOptions mediatorOptions, ApplicationOptions applicationOptions, ILogger? logger = null)
    {
        _mediatorController = mediatorController;
        _mediatorOptions = mediatorOptions;
        _applicationOptions = applicationOptions;
        _logger = logger?.ResolveLogger<Application>();
        RunStart();
        if (applicationOptions.StartWithCLI)
        {
            RunCLI();
        }
    }
    private void RunStart()
    {
        _logger?.Info("Started application with" + (_applicationOptions.StartWithCLI ? " CLI enabled" : "out CLI"));
        System.Console.WriteLine("Started Janus Mediator service application");
    }

    private void RunCLI()
    {
        System.Console.WriteLine("Welcome to the Janus Mediator CLI application!");
        System.Console.WriteLine(
            @"    __
___( o)>
\ <_. )
 `---'   "
            );
        System.Console.WriteLine("This duck is it for now, press any key...");
        System.Console.ReadKey();
    }


}
