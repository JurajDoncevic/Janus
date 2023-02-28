using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mask.Translation;

namespace Janus.Mask;
public abstract class MaskSchemaManager<TMaskSchema>
    : IComponentSchemaManager, IDelegatingSchemaManager
{
    private readonly MaskCommunicationNode _communicationNode;
    private readonly IMaskSchemaTranslator<TMaskSchema> _schemaTranslator;
    private readonly ILogger<MaskSchemaManager<TMaskSchema>>? _logger;
    private Option<DataSource> _currentSchema = Option<DataSource>.None;
    private Option<RemotePoint> _currentSchemaRemotePoint;

    public Option<RemotePoint> CurrentSchemaRemotePoint => _currentSchemaRemotePoint;

    public MaskSchemaManager(MaskCommunicationNode communicationNode, IMaskSchemaTranslator<TMaskSchema> schemaTranslator, ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _schemaTranslator = schemaTranslator;
        _logger = logger?.ResolveLogger<MaskSchemaManager<TMaskSchema>>();
    }

    /// <summary>
    /// Returns the current masked schema (masked output)
    /// </summary>
    /// <returns>Masked schema</returns>
    public Option<TMaskSchema> CurrentMaskedSchema
        => _currentSchema.Map(_schemaTranslator.Translate);

    public Option<DataSource> CurrentOutputSchema
        => _currentSchema;

    public async Task<Result<DataSource>> GetSchemaFrom(RemotePoint remotePoint)
        => (await Results.AsResult(async () =>
        {
            var result = await _communicationNode.SendSchemaRequest(remotePoint);

            return result;
        }))
        .Pass(
            r => _logger?.Info($"Successfully got schema named {r.Data.Name} ({r.Data.Version}) from {remotePoint}"),
            r => _logger?.Info($"Failed to get schema from {remotePoint} with message: {r.Message}")
            );

    public async Task<Result<DataSource>> LoadSchema(RemotePoint remotePoint)
        => (await Results.AsResult(async () =>
        {
            if (!_communicationNode.RemotePoints.Contains(remotePoint))
            {
                Results.OnFailure<DataSource>($"Remote point {remotePoint} is not registered. Can't load schema from unregistered remote point.");
            }

            var result = await _communicationNode.SendSchemaRequest(remotePoint);
            if (result)
            {
                _currentSchema = Option<DataSource>.Some(result.Data);
                _currentSchemaRemotePoint = Option<RemotePoint>.Some(remotePoint);
            }

            return result;
        }))
        .Pass(
            r => _logger?.Info($"Successfully loaded schema named {r.Data.Name} ({r.Data.Version}) from {remotePoint}"),
            r => _logger?.Info($"Failed to load schema from {remotePoint} with message: {r.Message}")
            );

    public async Task<Result<DataSource>> ReloadOutputSchema()
        => (await Results.AsResult(async () =>
        {
            if (!_currentSchemaRemotePoint)
            {
                Results.OnFailure<DataSource>("No current schema remote point set.");
            }
            var remotePoint = _currentSchemaRemotePoint.Value;
            if (!_communicationNode.RemotePoints.Contains(remotePoint))
            {
                Results.OnFailure<DataSource>($"Remote point {remotePoint} is not registered. Can't load schema from unregistered remote point.");
            }

            var result = await _communicationNode.SendSchemaRequest(remotePoint);
            if (result)
            {
                _currentSchema = Option<DataSource>.Some(result.Data);
                _currentSchemaRemotePoint = Option<RemotePoint>.Some(remotePoint);
            }

            return result;
        }))
        .Pass(
            r => _logger?.Info($"Successfully reloaded schema named {r.Data.Name} ({r.Data.Version}) from {_currentSchemaRemotePoint.Value}"),
            r => _logger?.Info($"Failed to reload schema with message: {r.Message}")
            );

    public Result UnloadSchema()
        => _currentSchemaRemotePoint.Match(
            rp => UnloadSchema(rp),
            () => Results.OnFailure("No schema currently loaded")
            )
            .Pass(
                r => _logger?.Info($"Successfully unloaded schema"),
                r => _logger?.Info($"Failed to unload schema with message: {r.Message}")
                );

    public Result UnloadSchema(RemotePoint remotePoint)
        => Results.AsResult(() =>
        {
            if (_currentSchemaRemotePoint && _currentSchemaRemotePoint.Value.Equals(remotePoint))
            {
                _currentSchema = Option<DataSource>.None;
                _currentSchemaRemotePoint = Option<RemotePoint>.None;
                return Results.OnSuccess("Schema unloaded");
            }
            else
            {
                return Results.OnFailure($"No schema loaded from remote point {remotePoint}");
            }
        })
        .Pass(
            r => _logger?.Info($"Successfully unloaded schema belonging to {remotePoint}"),
            r => _logger?.Info($"Failed to unload schema with message: {r.Message}")
            );
}
