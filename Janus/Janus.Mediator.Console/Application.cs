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
        System.Console.WriteLine($"This is Mediator {_mediatorOptions.NodeId}");
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
                conf.Message = "Choose an operation";
                conf.Items = new List<(string name, Func<Task<Result>> operation)>()
                    {
                                ("Send HELLO ping", DisplaySendHelloPing),
                                ("Get registered nodes", DisplayRegisteredRemotePoints),
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
            .Pass(r => System.Console.WriteLine($"{r.Message}. Got HELLO response on:{r.Data}"),
                  r => System.Console.WriteLine($"{r.Message}."))
            .Bind(r => Result.OnSuccess("Got response: " + r.ToString()));
    }
    private async Task<Result> DisplayRegisteredRemotePoints()
    {
        var nodeStrings =
            _mediatorController.GetRegisteredRemotePoints()
                .Select(rp => $"{rp.RemotePointType} with id {rp.NodeId} on {rp.Address}:{rp.Port}")
                .ToList();
        if(nodeStrings.Count > 0)
        {
            System.Console.WriteLine($"Registered remote points:\n{string.Join("\n", nodeStrings)}");
        }
        else
        {
            System.Console.WriteLine("No registered nodes");
        }
        return Result.OnSuccess();
    }
    private async Task<Result> DisplayRegisterRemotePoint()
    {
        System.Console.WriteLine("Enter remote point data");
        var address = Prompt.Input<string>("Target address");
        var port = Prompt.Input<int>("Target port");
        var result = await _mediatorController.RegisterRemotePoint(address, port);


        return result
            .Pass(r => System.Console.WriteLine($"{r.Message}. Got register response on:{r.Data}"),
                  r => System.Console.WriteLine($"{r.Message}."))
            .Bind(r => Result.OnSuccess("Got response: " + r.ToString()));
    }

    private async Task<Result> DisplayUnregisterNode()
    {
        System.Console.WriteLine("Enter remote point data");
        var nodeId = Prompt.Input<string>("Target node id");

        var targetRemotePoint = _mediatorController.GetRegisteredRemotePoints()
                                    .SingleOrDefault(rp => rp.NodeId.Equals(nodeId));

        if(targetRemotePoint != null)
        {
            var result = await _mediatorController.UnregisterRemotePoint(targetRemotePoint);

            return result
                .Pass(r => System.Console.WriteLine($"Unregister success. {r.Message}."))
                .Bind(r => Result.OnSuccess("Got response: " + r.ToString()));
        }
        else
        {
            System.Console.WriteLine($"Remote point with node id {nodeId} not found");
            return Result.OnFailure($"Remote point with node id {nodeId} not found");
        }

    }

    private async Task<Result> DisplayShutDown()
    {
        _logger?.Info("Exiting application and shutting down component");
        Environment.Exit(0);
        return Result.OnSuccess("Exiting application");
    }
}
