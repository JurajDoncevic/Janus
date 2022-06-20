using Janus.Mediator.Console.Displays;
using Janus.Mediator.Console.Options;
using Janus.Mediator.Core;
using Janus.Utils.Logging;
using Sharprompt;

namespace Janus.Mediator.Console;
public class Application
{
    private readonly MediatorController _mediatorController;
    private readonly MediatorOptions _mediatorOptions;
    private readonly ApplicationOptions _applicationOptions;
    private readonly ILogger<Application>? _logger;
    private readonly MainMenuDisplay _mainMenuDisplay;
    private readonly CliGreetingDisplay _cliGreetingDisplay;

    public Application(MediatorController mediatorController, MediatorOptions mediatorOptions, ApplicationOptions applicationOptions, ILogger? logger = null)
    {
        _mediatorController = mediatorController;
        _mediatorOptions = mediatorOptions;
        _applicationOptions = applicationOptions;
        _logger = logger?.ResolveLogger<Application>();
        RunStart();
        if (applicationOptions.StartWithCLI)
        {
            _cliGreetingDisplay = new CliGreetingDisplay(_mediatorController, _mediatorOptions);
            _mainMenuDisplay = new MainMenuDisplay(_mediatorController, logger);
            RunCLI();
        }
    }
    private void RunStart()
    {
        _logger?.Info("Starting application with" + (_applicationOptions.StartWithCLI ? " CLI enabled" : "out CLI" + $". With node id {_mediatorOptions.NodeId} on port {_mediatorOptions.ListenPort}"));
        System.Console.WriteLine("Started Janus Mediator service");
    }

    private void RunCLI()
    {
        _cliGreetingDisplay.Show().Wait();
        _mainMenuDisplay.Show().Wait();
    }
}
