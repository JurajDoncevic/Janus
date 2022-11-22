using FunctionalExtensions.Base.Resulting;
using Janus.Logging;

namespace Janus.Wrapper.Sqlite.ConsoleApp.Displays;
public class AllRegisteredRemotePointsDisplay : BaseDisplay
{
    ILogger<AllRegisteredRemotePointsDisplay>? _logger;
    public AllRegisteredRemotePointsDisplay(SqliteWrapperController wrapperController, ILogger? logger) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<AllRegisteredRemotePointsDisplay>();
    }

    public override string Title => "ALL REGISTERED REMOTE POINTS";

    protected override async Task<Result> Display()
    {
        var nodeStrings =
            _wrapperController.GetRegisteredRemotePoints()
                .Select(rp => $"{rp.RemotePointType} with id {rp.NodeId} on {rp.Address}:{rp.Port}")
                .ToList();
        if (nodeStrings.Count > 0)
        {
            System.Console.WriteLine($"Registered remote points:\n{string.Join("\n", nodeStrings)}");
        }
        else
        {
            System.Console.WriteLine("No registered nodes");
        }
        return await Task.FromResult(Results.OnSuccess());
    }
}
