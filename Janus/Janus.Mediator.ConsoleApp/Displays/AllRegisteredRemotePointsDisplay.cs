﻿using FunctionalExtensions.Base.Resulting;
using Janus.Logging;

namespace Janus.Mediator.ConsoleApp.Displays;
public class AllRegisteredRemotePointsDisplay : BaseDisplay
{
    ILogger<AllRegisteredRemotePointsDisplay>? _logger;
    public AllRegisteredRemotePointsDisplay(MediatorManager MediatorManager, ILogger? logger) : base(MediatorManager)
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
        return await Task.FromResult(Results.OnSuccess());
    }
}
