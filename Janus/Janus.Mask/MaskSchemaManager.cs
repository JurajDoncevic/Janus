using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;

namespace Janus.Mask;
public class MaskSchemaManager : IComponentSchemaManager, IDelegatingSchemaManager
{
    private readonly MaskCommunicationNode _communicationNode;
    private readonly ILogger<MaskSchemaManager>? _logger;
    private Option<DataSource> _currentSchema = Option<DataSource>.None;
    private Option<RemotePoint> _currentSchemaRemotePoint;

    public MaskSchemaManager(MaskCommunicationNode communicationNode, ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _logger = logger?.ResolveLogger<MaskSchemaManager>();
    }

    public Option<DataSource> GetCurrentOutputSchema()
        => _currentSchema;

    public async Task<Result<DataSource>> GetSchemaFrom(RemotePoint remotePoint)
        => await Results.AsResult(async () =>
        {
            var result = await _communicationNode.SendSchemaRequest(remotePoint);

            return result;
        });

    public async Task<Result<DataSource>> LoadSchema(RemotePoint remotePoint)
        => await Results.AsResult(async () =>
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
        });

    public async Task<Result<DataSource>> ReloadOutputSchema()
        => await Results.AsResult(async () =>
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
        });

    public Result UnloadSchema()
        => _currentSchemaRemotePoint.Match(
            rp => UnloadSchema(rp),
            () => Results.OnFailure("No schema currently loaded")
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
        });
}
