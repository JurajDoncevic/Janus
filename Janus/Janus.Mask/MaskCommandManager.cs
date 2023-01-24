using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;

namespace Janus.Mask;
public sealed class MaskCommandManager : IDelegatingCommandManager
{
    private readonly MaskCommunicationNode _communicationNode;
    private readonly MaskSchemaManager _schemaManager;
    private readonly ILogger<MaskCommandManager>? _logger;

    public MaskCommandManager(MaskCommunicationNode communicationNode, MaskSchemaManager schemaManager, ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _schemaManager = schemaManager;
        _logger = logger?.ResolveLogger<MaskCommandManager>();
    }

    public async Task<Result> RunCommand(BaseCommand command)
        => (await Results.AsResult(async () =>
        {
            if (!_schemaManager.CurrentSchemaRemotePoint)
            {
                return Results.OnFailure("No schema remote point loaded");
            }

            var result = await _communicationNode.SendCommandRequest(command, _schemaManager.CurrentSchemaRemotePoint.Value);

            return result;
        }))
        .Pass(
            r => _logger?.Info($"Successfully ran {command.CommandType} command {command.Name}"),
            r => _logger?.Info($"Failed to run {command.CommandType} command {command.Name} with message: {r.Message}")
            );

    public async Task<Result> RunCommandOnComponent(BaseCommand command, RemotePoint remotePoint)
        => (await Results.AsResult(async () =>
        {
            if (!_communicationNode.RemotePoints.Contains(remotePoint))
            {
                return Results.OnFailure<TabularData>($"Remote point {remotePoint} is not registered.");
            }

            var result = await _communicationNode.SendCommandRequest(command, remotePoint);

            return result;
        }))
        .Pass(
            r => _logger?.Info($"Successfully ran {command.CommandType} command {command.Name}"),
            r => _logger?.Info($"Failed to run {command.CommandType} command {command.Name} with message: {r.Message}")
            );
}
