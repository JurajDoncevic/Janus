using Janus.Mediator.Core;
using Janus.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mediator.ConsoleApp.Displays;
public class AllRegisteredRemotePointsDisplay : BaseDisplay
{
    ILogger<AllRegisteredRemotePointsDisplay>? _logger;
    public AllRegisteredRemotePointsDisplay(MediatorController mediatorController, ILogger? logger) : base(mediatorController)
    {
        _logger = logger?.ResolveLogger<AllRegisteredRemotePointsDisplay>();
    }

    public override string Title => "ALL REGISTERED REMOTE POINTS";

    protected override async Task<Result> Display()
    {
        var nodeStrings =
            _mediatorController.GetRegisteredRemotePoints()
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
        return await Task.FromResult(Result.OnSuccess());
    }
}
