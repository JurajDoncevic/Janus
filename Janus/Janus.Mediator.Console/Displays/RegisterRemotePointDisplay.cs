using Janus.Mediator.Core;
using Janus.Utils.Logging;
using Sharprompt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.Console.Displays;
public class RegisterRemotePointDisplay : BaseDisplay
{

    private readonly ILogger<RegisterRemotePointDisplay>? _logger;

    public RegisterRemotePointDisplay(MediatorController mediatorController, ILogger? logger = null) : base(mediatorController)
    {
        _logger = logger?.ResolveLogger<RegisterRemotePointDisplay>();
    }

    public override string Title => "REGISTER REMOTE POINT";

    protected async override Task<Result> Display()
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
}
