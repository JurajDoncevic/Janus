using FunctionalExtensions.Base.Resulting;
using Janus.Logging;
using Sharprompt;

namespace Janus.Mediator.ConsoleApp.Displays;
public class MainMenuDisplay : BaseDisplay
{
    private readonly ILogger<MainMenuDisplay>? _logger;

    private readonly SendHelloPingDisplay _sendHelloPingDisplay;
    private readonly AllRegisteredRemotePointsDisplay _allRegisteredRemotePointsDisplay;
    private readonly UnregisterNodeDisplay _unregisterNodeDisplay;
    private readonly RegisterRemotePointDisplay _registerRemotePointDisplay;
    private readonly SchemaInferrenceSelectionDisplay _schemaInferrenceSelectionDisplay;
    private readonly InputSchemataDisplay _inputSchemataDisplay;
    private readonly ShutDownDisplay _shutDownDisplay;

    public MainMenuDisplay(MediatorController mediatorController, ILogger? logger = null) : base(mediatorController)
    {
        if (mediatorController is null)
        {
            throw new ArgumentNullException(nameof(mediatorController));
        }

        _logger = logger?.ResolveLogger<MainMenuDisplay>();

        _sendHelloPingDisplay = new SendHelloPingDisplay(mediatorController, logger);
        _allRegisteredRemotePointsDisplay = new AllRegisteredRemotePointsDisplay(mediatorController, logger);
        _unregisterNodeDisplay = new UnregisterNodeDisplay(mediatorController, logger);
        _registerRemotePointDisplay = new RegisterRemotePointDisplay(mediatorController, logger);
        _schemaInferrenceSelectionDisplay = new SchemaInferrenceSelectionDisplay(mediatorController, logger);
        _inputSchemataDisplay = new InputSchemataDisplay(mediatorController, logger);
        _shutDownDisplay = new ShutDownDisplay(mediatorController, logger);

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
                                ("Get input schemata", async () => await _inputSchemataDisplay.Show()),
                                ("Set remote points for input schemata", async () => await _schemaInferrenceSelectionDisplay.Show()),
                                ("Exit and shutdown node", async () => await _shutDownDisplay.Show())
                };
            conf.TextSelector = (item) => item.name;
        });
        var result = await mainMenuSelection.command.Invoke();
        return result;
    }
}
