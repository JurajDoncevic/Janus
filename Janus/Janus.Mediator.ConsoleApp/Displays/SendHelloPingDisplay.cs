using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Logging;
using Sharprompt;

namespace Janus.Mediator.ConsoleApp.Displays;
public class SendHelloPingDisplay : BaseDisplay
{
    private readonly ILogger<SendHelloPingDisplay>? _logger;
    public SendHelloPingDisplay(MediatorManager MediatorManager, ILogger? logger = null) : base(MediatorManager)
    {
        if (MediatorManager is null)
        {
            throw new ArgumentNullException(nameof(MediatorManager));
        }

        _logger = logger?.ResolveLogger<SendHelloPingDisplay>();
    }

    public override string Title => "SEND HELLO";

    protected override async Task<Result> Display()
    {
        System.Console.WriteLine("Enter HELLO ping data");
        var address = Prompt.Input<string>("Target address");
        var port = Prompt.Input<int>("Target port");

        var result = await _mediatorController.SendHello(address, port);

        return result
            .Pass(r => System.Console.WriteLine($"{r.Message}. Got HELLO response on:{r.Data}"),
                  r => System.Console.WriteLine($"HELLO failed. {r.Message}."))
            .Bind(r => Results.OnSuccess("Got response: " + r.ToString()));
    }
}
