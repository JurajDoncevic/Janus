using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Logging;
using Sharprompt;

namespace Janus.Wrapper.Sqlite.ConsoleApp.Displays;
public class UnregisterNodeDisplay : BaseDisplay
{
    private readonly ILogger<UnregisterNodeDisplay>? _logger;

    public UnregisterNodeDisplay(SqliteWrapperController mediatorController, ILogger? logger = null) : base(mediatorController)
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
                .Bind(r => Results.OnSuccess("Got response: " + r.ToString()));
        }
        else
        {
            System.Console.WriteLine($"Remote point with node id {nodeId} not found");
            return Results.OnFailure($"Remote point with node id {nodeId} not found");
        }
    }
}
