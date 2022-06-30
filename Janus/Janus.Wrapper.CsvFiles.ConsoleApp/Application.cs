using Janus.Communication.Remotes;
using Janus.Wrapper.CsvFiles.ConsoleApp.Displays;
using Janus.Wrapper.CsvFiles.ConsoleApp.Options;
using Janus.Wrapper.Core;
using Janus.Utils.Logging;
using Sharprompt;

namespace Janus.Wrapper.CsvFiles.ConsoleApp;
public class Application
{
    private readonly CsvFilesWrapperController _wrapperController;
    private readonly WrapperOptions _wrapperOptions;
    private readonly ApplicationOptions _applicationOptions;
    private readonly ILogger<Application>? _logger;
    private readonly MainMenuDisplay? _mainMenuDisplay;
    private readonly CliGreetingDisplay? _cliGreetingDisplay;

    public Application(CsvFilesWrapperController wrapperController, WrapperOptions wrapperOptions, ApplicationOptions applicationOptions, ILogger? logger = null)
    {
        _wrapperController = wrapperController;
        _wrapperOptions = wrapperOptions;
        _applicationOptions = applicationOptions;
        _logger = logger?.ResolveLogger<Application>();
        RunStart();
        if (applicationOptions.StartWithCLI)
        {
            _cliGreetingDisplay = new CliGreetingDisplay(_wrapperController, _wrapperOptions);
            _mainMenuDisplay = new MainMenuDisplay(_wrapperController, logger);
            RunCLI();
        }
    }
    private void RunStart()
    {
        _logger?.Info("Starting application with" + (_applicationOptions.StartWithCLI ? " CLI enabled" : "out CLI" + $". With node id {_wrapperOptions.NodeId} on port {_wrapperOptions.ListenPort}"));
        System.Console.WriteLine("Started Janus Wrapper service");
        _logger?.Info("Attempting to connect to startup remote nodes: {0}", string.Join(",", _wrapperOptions.StartupRemotePoints));

        var results =
        _wrapperOptions.StartupRemotePoints
            .AsParallel()
            .Select(async rp => (rp.Address, rp.Port, Result: await _wrapperController.RegisterRemotePoint(rp.Address, rp.Port)))
            .ToList();
        
        results
            .ForEach(async r =>
            {

                var result = await r;
                if (result.Result)
                {
                    Console.WriteLine($"Successfully registered to {result.Result.Data}");
                }
                else
                {
                    Console.WriteLine($"Failed to register to {result.Address}:{result.Port}");
                }
            });
        
        var successResults = results.Where(r => r.Result.Result).Select(r => r.Result).ToList();
        var failResults = results.Where(r => !r.Result.Result).Select(r => r.Result).ToList();

        if (successResults.Count > 0)
        {
            _logger?.Info("Registered to following nodes on startup: {0}", string.Join(",", successResults.Select(r => r.Result)));
        }
        if (failResults.Count > 0)
        {
            _logger?.Info("Failed to register to the following nodes on startup: {0}", string.Join(",", failResults.Select(r => r.Address + ":" + r.Port.ToString())));
        }
    }

    private void RunCLI()
    {
        _cliGreetingDisplay?.Show().Wait();

        while(true)
            _mainMenuDisplay?.Show().Wait();
    }
}
