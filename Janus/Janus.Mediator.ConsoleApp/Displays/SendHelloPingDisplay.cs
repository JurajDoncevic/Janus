using Janus.Mediator.Core;
using Janus.Utils.Logging;
using Sharprompt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
