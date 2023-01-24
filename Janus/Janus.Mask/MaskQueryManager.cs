using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;

namespace Janus.Mask;
public class MaskQueryManager : IDelegatingQueryManager
{
    private readonly MaskCommunicationNode _communicationNode;
    private readonly MaskSchemaManager _schemaManager;
    private readonly ILogger<MaskQueryManager>? _logger;

    public MaskQueryManager(MaskCommunicationNode communicationNode, MaskSchemaManager schemaManager, ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _schemaManager = schemaManager;
        _logger = logger?.ResolveLogger<MaskQueryManager>();
    }

    public async Task<Result<TabularData>> RunQuery(Query query)
        => (await Results.AsResult<TabularData>(async () =>
        {
            if (!_schemaManager.CurrentSchemaRemotePoint)
            {
                return Results.OnFailure<TabularData>("No schema remote point loaded");
            }

            var result = await _communicationNode.SendQueryRequest(query, _schemaManager.CurrentSchemaRemotePoint.Value);

            return result;
        }))
        .Pass(
            r => _logger?.Info($"Successful query {query.Name} run"),
            r => _logger?.Info($"Failed query {query.Name} run with message: {r.Message}")
            );

    public async Task<Result<TabularData>> RunQueryOn(Query query, RemotePoint remotePoint)
        => (await Results.AsResult<TabularData>(async () =>
        {
            if (!_communicationNode.RemotePoints.Contains(remotePoint))
            {
                return Results.OnFailure<TabularData>($"Remote point {remotePoint} is not registered.");
            }

            var result = await _communicationNode.SendQueryRequest(query, remotePoint);

            return result;
        }))
        .Pass(
            r => _logger?.Info($"Successful query {query.Name} run"),
            r => _logger?.Info($"Failed query {query.Name} run with message: {r.Message}")
            );
}
