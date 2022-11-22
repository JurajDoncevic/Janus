using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Logging;
using Sharprompt;

namespace Janus.Wrapper.Sqlite.ConsoleApp.Displays;
public class SendHelloPingDisplay : BaseDisplay
{
    private readonly ILogger<SendHelloPingDisplay>? _logger;
    public SendHelloPingDisplay(SqliteWrapperController wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        if (wrapperController is null)
        {
            throw new ArgumentNullException(nameof(wrapperController));
        }

        _logger = logger?.ResolveLogger<SendHelloPingDisplay>();
    }

    public override string Title => "SEND HELLO";

    protected override async Task<Result> Display()
    {
        System.Console.WriteLine("Enter HELLO ping data");
        var address = Prompt.Input<string>("Target address");
        var port = Prompt.Input<int>("Target port");

        var result = await _wrapperController.SendHello(address, port);

        return result
            .Pass(r => System.Console.WriteLine($"{r.Message}. Got HELLO response on:{r.Data}"),
                  r => System.Console.WriteLine($"HELLO failed. {r.Message}."))
            .Bind(r => Results.OnSuccess("Got response: " + r.ToString()));
    }
}
