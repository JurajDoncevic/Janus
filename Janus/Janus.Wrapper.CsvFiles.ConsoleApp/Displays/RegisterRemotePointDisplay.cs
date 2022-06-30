using Janus.Wrapper.Core;
using Janus.Utils.Logging;
using Sharprompt;

namespace Janus.Wrapper.CsvFiles.ConsoleApp.Displays;
public class RegisterRemotePointDisplay : BaseDisplay
{

    private readonly ILogger<RegisterRemotePointDisplay>? _logger;

    public RegisterRemotePointDisplay(CsvFilesWrapperController wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<RegisterRemotePointDisplay>();
    }

    public override string Title => "REGISTER REMOTE POINT";

    protected async override Task<Result> Display()
    {
        System.Console.WriteLine("Enter remote point data");
        var address = Prompt.Input<string>("Target address");
        var port = Prompt.Input<int>("Target port");
        var result = await _wrapperController.RegisterRemotePoint(address, port);


        return result
            .Pass(r => System.Console.WriteLine($"{r.Message}. Got register response on:{r.Data}"),
                  r => System.Console.WriteLine($"{r.Message}."))
            .Bind(r => Result.OnSuccess("Got response: " + r.ToString()));
    }
}
