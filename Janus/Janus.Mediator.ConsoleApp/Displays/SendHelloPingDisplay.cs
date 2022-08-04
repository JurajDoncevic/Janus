using Janus.Mediator;
using Janus.Logging;
using Sharprompt;
using FunctionalExtensions.Base.Results;
using FunctionalExtensions.Base;

namespace Janus.Mediator.ConsoleApp.Displays;
public class SendHelloPingDisplay : BaseDisplay
{
    private readonly ILogger<SendHelloPingDisplay>? _logger;
    public SendHelloPingDisplay(MediatorController mediatorController!!, ILogger? logger = null) : base(mediatorController)
    {
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
            .Bind(r => Result.OnSuccess("Got response: " + r.ToString()));
    }
}
