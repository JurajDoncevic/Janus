using FunctionalExtensions.Base.Resulting;
using Janus.Logging;
using Sharprompt;

namespace Janus.Wrapper.Sqlite.ConsoleApp.Displays;
public class MainMenuDisplay : BaseDisplay
{
    private readonly ILogger<MainMenuDisplay>? _logger;

    private readonly SendHelloPingDisplay _sendHelloPingDisplay;
    private readonly AllRegisteredRemotePointsDisplay _allRegisteredRemotePointsDisplay;
    private readonly UnregisterNodeDisplay _unregisterNodeDisplay;
    private readonly RegisterRemotePointDisplay _registerRemotePointDisplay;
    private readonly GetCurrentSchemaDisplay _getCurrentSchemaDisplay;
    private readonly ShutDownDisplay _shutDownDisplay;

    public MainMenuDisplay(SqliteWrapperManager wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        if (wrapperController is null)
        {
            throw new ArgumentNullException(nameof(wrapperController));
        }

        _logger = logger?.ResolveLogger<MainMenuDisplay>();

        _sendHelloPingDisplay = new SendHelloPingDisplay(wrapperController, logger);
        _allRegisteredRemotePointsDisplay = new AllRegisteredRemotePointsDisplay(wrapperController, logger);
        _unregisterNodeDisplay = new UnregisterNodeDisplay(wrapperController, logger);
        _registerRemotePointDisplay = new RegisterRemotePointDisplay(wrapperController, logger);
        _getCurrentSchemaDisplay = new GetCurrentSchemaDisplay(wrapperController, logger);
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
                                ("Get current schema", async () => await _getCurrentSchemaDisplay.Show()),
                                ("Exit and shutdown node", async () => await _shutDownDisplay.Show())
                };
            conf.TextSelector = (item) => item.name;
        });
        var result = await mainMenuSelection.command.Invoke();
        return result;
    }
}
