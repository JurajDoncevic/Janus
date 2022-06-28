using Janus.Wrapper.Core;
using Janus.Utils.Logging;
using Sharprompt;

namespace Janus.Wrapper.CsvFiles.ConsoleApp.Displays;
public class MainMenuDisplay : BaseDisplay
{
    private readonly ILogger<MainMenuDisplay>? _logger;

    private readonly SendHelloPingDisplay _sendHelloPingDisplay;
    private readonly AllRegisteredRemotePointsDisplay _allRegisteredRemotePointsDisplay;
    private readonly UnregisterNodeDisplay _unregisterNodeDisplay;
    private readonly RegisterRemotePointDisplay _registerRemotePointDisplay;
    private readonly CurrentSchemaDisplay _currentSchemaDisplay;
    private readonly ReloadSchemaDisplay _reloadSchemaDisplay;
    private readonly ShutDownDisplay _shutDownDisplay;

    public MainMenuDisplay(WrapperController wrapperController!!, ILogger? logger = null) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<MainMenuDisplay>();

        _sendHelloPingDisplay = new SendHelloPingDisplay(wrapperController, logger);
        _allRegisteredRemotePointsDisplay = new AllRegisteredRemotePointsDisplay(wrapperController, logger);
        _unregisterNodeDisplay = new UnregisterNodeDisplay(wrapperController, logger);
        _registerRemotePointDisplay = new RegisterRemotePointDisplay(wrapperController, logger);
        _currentSchemaDisplay = new CurrentSchemaDisplay(wrapperController, logger);
        _reloadSchemaDisplay = new ReloadSchemaDisplay(wrapperController, logger);
        _shutDownDisplay = new ShutDownDisplay(wrapperController, logger);


    }

    public override string Title => "MAIN MENU";

    protected override async Task<Result> Display()
    {
        var mainMenuSelection =
        Prompt.Select<(string name, Func<Task<Result>> command)>(conf =>
        {
            conf.Message = "Choose an operation";
            conf.Items = new List<(string name, Func<Task<Result>> command)>()
                {
                                ("Send HELLO ping", async () => await _sendHelloPingDisplay.Show()),
                                ("Get registered nodes", async () => await _allRegisteredRemotePointsDisplay.Show()),
                                ("Register new remote point", async () => await _registerRemotePointDisplay.Show()),
                                ("Unregister node", async () => await _unregisterNodeDisplay.Show()),
                                ("Get current schema", async () => await _currentSchemaDisplay.Show()),
                                ("Reload current schema", async () => await _reloadSchemaDisplay.Show()),
                                ("Exit and shutdown node", async () => await _shutDownDisplay.Show())
                };
            conf.TextSelector = (item) => item.name;
        });
        var result = await mainMenuSelection.command.Invoke();
        return result;
    }
}
