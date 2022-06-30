using Janus.Wrapper.Core;
using Janus.Utils.Logging;
using Sharprompt;

namespace Janus.Wrapper.CsvFiles.ConsoleApp.Displays;
public class UnregisterNodeDisplay : BaseDisplay
{
    private readonly ILogger<UnregisterNodeDisplay>? _logger;

    public UnregisterNodeDisplay(CsvFilesWrapperController wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<UnregisterNodeDisplay>();
    }

    public override string Title => "UNREGISTER NODE";

    protected async override Task<Result> Display()
    {
        System.Console.WriteLine("Enter remote point data");
        var nodeId = Prompt.Input<string>("Target node id");

        var targetRemotePoint = _wrapperController.GetRegisteredRemotePoints()
                                    .SingleOrDefault(rp => rp.NodeId.Equals(nodeId));

        if (targetRemotePoint != null)
        {
            var result = await _wrapperController.UnregisterRemotePoint(targetRemotePoint);

            return result
                .Pass(r => System.Console.WriteLine($"Unregister success. {r.Message}."))
                .Bind(r => Result.OnSuccess("Got response: " + r.ToString()));
        }
        else
        {
            System.Console.WriteLine($"Remote point with node id {nodeId} not found");
            return Result.OnFailure($"Remote point with node id {nodeId} not found");
        }
    }
}
