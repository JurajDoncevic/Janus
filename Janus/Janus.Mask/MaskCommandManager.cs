using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.DataModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mask.MaskedCommandModel;
using Janus.Mask.MaskedSchemaModel;
using Janus.Mask.Translation;

namespace Janus.Mask;
public abstract class MaskCommandManager<TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedSelection, TMaskedInstantiation, TMaskedMutation, TMaskedSchema>
    : IDelegatingCommandManager
    where TMaskedDeleteCommand : MaskedDelete<TMaskedSelection>
    where TMaskedInsertCommand : MaskedInsert<TMaskedInstantiation>
    where TMaskedUpdateCommand : MaskedUpdate<TMaskedSelection, TMaskedMutation>
    where TMaskedSchema : MaskedDataSource
{
    private readonly MaskCommunicationNode _communicationNode;
    private readonly MaskSchemaManager<TMaskedSchema> _schemaManager;
    private readonly IMaskCommandTranslator<TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedSelection, TMaskedMutation, TMaskedInstantiation> _commandTranslator;
    private readonly ILogger<MaskCommandManager<TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedSelection, TMaskedInstantiation, TMaskedMutation, TMaskedSchema>>? _logger;

    public MaskCommandManager(
        MaskCommunicationNode communicationNode,
        MaskSchemaManager<TMaskedSchema> schemaManager,
        IMaskCommandTranslator<TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedSelection, TMaskedMutation, TMaskedInstantiation> commandTranslator,
        ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _schemaManager = schemaManager;
        _commandTranslator = commandTranslator;
        _logger = logger?.ResolveLogger<MaskCommandManager<TMaskedDeleteCommand, TMaskedInsertCommand, TMaskedUpdateCommand, TMaskedSelection, TMaskedInstantiation, TMaskedMutation, TMaskedSchema>>();
    }

    public abstract Task<Result> RunCommand(TMaskedDeleteCommand command);
    public abstract Task<Result> RunCommand(TMaskedInsertCommand command);
    public abstract Task<Result> RunCommand(TMaskedUpdateCommand command);

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
