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
        DisplayMainMenu();
    }

    private void DisplayMainMenu()
    {
        _logger?.Info("Showing main menu to CLI user");
        while (true)
        {
            var mainMenuSelection =
            Prompt.Select<(string name, Func<Task<Result>> operation)>(conf =>
            {
                conf.Message = "Choose and operation";
                conf.Items = new List<(string name, Func<Task<Result>> operation)>()
                    {
                                ("Send HELLO ping", DisplaySendHelloPing),
                                ("Get registered nodes", DisplayRegisteredNodes),
                                ("Register new remote point", DisplayRegisterRemotePoint),
                                ("Unregister node", DisplayUnregisterNode),
                                ("Exit and shutdown node", DisplayShutDown)
                    };
                conf.TextSelector = (item) => item.name;
            });
            mainMenuSelection.operation.Invoke().Wait();
        }


    }

    private async Task<Result> DisplaySendHelloPing()
    {
        System.Console.WriteLine("Enter HELLO ping data");
        var address = Prompt.Input<string>("Target address");
        var port = Prompt.Input<int>("Target port");
        var result = await _mediatorController.SendHello(address, port);

        
        return result
            .Pass(r => System.Console.WriteLine($"{result.Message}. Got response on:{r.Data}"))
            .Bind(r => Result.OnSuccess("Got response: " + r.ToString()));
    }
    private async Task<Result> DisplayRegisteredNodes()
    {
        throw new NotImplementedException();
    }
    private async Task<Result> DisplayRegisterRemotePoint()
    {
        throw new NotImplementedException();
    }

    private async Task<Result> DisplayUnregisterNode()
    {
        throw new NotImplementedException();
    }

    private async Task<Result> DisplayShutDown()
    {
        _logger?.Info("Exiting application and shutting down component");
        Environment.Exit(0);
        return Result.OnSuccess("Exiting application");
    }
}
