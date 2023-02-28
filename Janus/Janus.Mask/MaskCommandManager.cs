using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mask.LocalCommanding;
using Janus.Mask.Translation;

namespace Janus.Mask;
public abstract class MaskCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TInstantiation, TMutation, TMaskSchema> 
    : IDelegatingCommandManager
    where TDeleteCommand : LocalDelete<TSelection>
    where TInsertCommand : LocalInsert<TInstantiation>
    where TUpdateCommand : LocalUpdate<TSelection, TMutation>
{
    private readonly MaskCommunicationNode _communicationNode;
    private readonly MaskSchemaManager<TMaskSchema> _schemaManager;
    private readonly IMaskCommandTranslator<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation> _commandTranslator;
    private readonly ILogger<MaskCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TInstantiation, TMutation, TMaskSchema>>? _logger;

    public MaskCommandManager(
        MaskCommunicationNode communicationNode,
        MaskSchemaManager<TMaskSchema> schemaManager,
        IMaskCommandTranslator<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TMutation, TInstantiation> commandTranslator,
        ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _schemaManager = schemaManager;
        _commandTranslator = commandTranslator;
        _logger = logger?.ResolveLogger<MaskCommandManager<TDeleteCommand, TInsertCommand, TUpdateCommand, TSelection, TInstantiation, TMutation, TMaskSchema>>();
    }

    public abstract Task<Result> RunCommand(TDeleteCommand command);
    public abstract Task<Result> RunCommand(TInsertCommand command);
    public abstract Task<Result> RunCommand(TUpdateCommand command);

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
