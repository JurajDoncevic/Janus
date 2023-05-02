using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mediation;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediator;

/// <summary>
/// Command manager for a mediator component
/// </summary>
public sealed class MediatorCommandManager : IDelegatingCommandManager, IMediatingCommandManager<MediatorSchemaManager>
{
    private readonly MediatorCommunicationNode _communicationNode;
    private readonly ILogger<MediatorCommandManager>? _logger;

    public MediatorCommandManager(MediatorCommunicationNode communicationNode, ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _logger = logger?.ResolveLogger<MediatorCommandManager>();
    }

    /// <summary>
    /// Runs a mediated command over other remote nodes
    /// </summary>
    /// <param name="command">Command on the mediated data source</param>
    /// <param name="schemaManager">Current schema manager</param>
    /// <returns>Operation result</returns>
    public async Task<Result> RunCommand(BaseCommand command, MediatorSchemaManager schemaManager)
        => (await Results.AsResult(async () =>
        {
            var currentMediatedSchema = schemaManager.CurrentMediatedSchema;
            if (!currentMediatedSchema)
            {
                return Results.OnFailure($"No mediated schema generated");
            }

            var currentMediation = schemaManager.CurrentMediation;
            if (!currentMediation)
            {
                return Results.OnFailure($"No mediation created");
            }

            var commandValidation = command.IsValidForDataSource(currentMediatedSchema.Value);
            if (!commandValidation)
            {
                return Results.OnFailure($"Command {command.Name} is not valid on mediated schema {currentMediatedSchema.Value.Name}: {commandValidation.Message}");
            }

            return await (command switch
            {
                { CommandType: CommandTypes.DELETE } => RunDeleteCommand((DeleteCommand)command, currentMediatedSchema.Value, currentMediation.Value, schemaManager.RemotePointWithLoadedDataSourceName),
                { CommandType: CommandTypes.INSERT } => RunInsertCommand((InsertCommand)command, currentMediatedSchema.Value, currentMediation.Value, schemaManager.RemotePointWithLoadedDataSourceName),
                { CommandType: CommandTypes.UPDATE } => RunUpdateCommand((UpdateCommand)command, currentMediatedSchema.Value, currentMediation.Value, schemaManager.RemotePointWithLoadedDataSourceName),
                _ => Task.FromResult(Results.OnFailure($"Unknown command {command.Name} type given for running."))
            });
        })).Pass(
            r => _logger?.Info($"Successfully ran {command.CommandType} command {command.Name}"),
            r => _logger?.Info($"Failed to run {command.CommandType} command {command.Name} with message: {r.Message}")
            );

    private async Task<Result> RunUpdateCommand(UpdateCommand command, DataSource mediatedSchema, DataSourceMediation dataSourceMediation, IReadOnlyDictionary<string, RemotePoint> remotePointsWithDataSourceNames)
        => await Results.AsResult(async () =>
        {
            var commandMediationResult = CommandModelMediation.MediateCommand(command, mediatedSchema, dataSourceMediation);
            if (!commandMediationResult)
            {
                return Results.OnFailure($"Failed to mediate update command: {commandMediationResult.Message}");
            }
            var commandMediation = commandMediationResult.Data;

            var targetRemotePoint = remotePointsWithDataSourceNames[commandMediation.TargetDataSourceId.DataSourceName];

            var commandRunResult = await _communicationNode.SendCommandRequest(command, targetRemotePoint);

            return commandRunResult;
        });

    private async Task<Result> RunDeleteCommand(DeleteCommand command, DataSource mediatedSchema, DataSourceMediation dataSourceMediation, IReadOnlyDictionary<string, RemotePoint> remotePointsWithDataSourceNames)
        => await Results.AsResult(async () =>
        {
            var commandMediationResult = CommandModelMediation.MediateCommand(command, mediatedSchema, dataSourceMediation);
            if (!commandMediationResult)
            {
                return Results.OnFailure($"Failed to mediate delete command: {commandMediationResult.Message}");
            }
            var commandMediation = commandMediationResult.Data;

            var targetRemotePoint = remotePointsWithDataSourceNames[commandMediation.TargetDataSourceId.DataSourceName];

            var commandRunResult = await _communicationNode.SendCommandRequest(command, targetRemotePoint);

            return commandRunResult;
        });

    private async Task<Result> RunInsertCommand(InsertCommand command, DataSource mediatedSchema, DataSourceMediation dataSourceMediation, IReadOnlyDictionary<string, RemotePoint> remotePointsWithDataSourceNames)
        => await Results.AsResult(async () =>
        {
            var commandMediationResult = CommandModelMediation.MediateCommand(command, mediatedSchema, dataSourceMediation);
            if (!commandMediationResult)
            {
                return Results.OnFailure($"Failed to mediate insert command: {commandMediationResult.Message}");
            }
            var commandMediation = commandMediationResult.Data;

            var targetRemotePoint = remotePointsWithDataSourceNames[commandMediation.TargetDataSourceId.DataSourceName];

            var commandRunResult = await _communicationNode.SendCommandRequest(command, targetRemotePoint);

            return commandRunResult;
        });

    public async Task<Result> RunCommandOnComponent(BaseCommand command, RemotePoint remotePoint)
        => (await Results.AsResult(async () =>
        {
            var commandResult = await _communicationNode.SendCommandRequest(command, remotePoint);

            return commandResult;
        })).Pass(
            r => _logger?.Info($"Successfully ran command {command.Name} on remote point {remotePoint}."),
            r => _logger?.Info($"Failed to run command {command.Name} on remote point {remotePoint}.")
            );
}
