using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mask.LocalDataModel;
using Janus.Mask.LocalQuerying;
using Janus.Mask.Translation;

namespace Janus.Mask;
public abstract class MaskQueryManager<TLocalQuery, TStartingWith, TSelection, TJoining, TProjection, TMaskSchema>
    : IDelegatingQueryManager
    where TLocalQuery : LocalQuery<TStartingWith, TSelection, TJoining, TProjection>
{
    private readonly MaskCommunicationNode _communicationNode;
    private readonly MaskSchemaManager<TMaskSchema> _schemaManager;
    private readonly IMaskQueryTranslator<TLocalQuery, TStartingWith, TSelection, TJoining, TProjection> _queryTranslator;

    private readonly ILogger<MaskQueryManager<TLocalQuery, TStartingWith, TSelection, TJoining, TProjection, TMaskSchema>>? _logger;

    public MaskQueryManager(
        MaskCommunicationNode communicationNode,
        MaskSchemaManager<TMaskSchema> schemaManager,
        IMaskQueryTranslator<TLocalQuery, TStartingWith, TSelection, TJoining, TProjection> queryTranslator,
        ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _schemaManager = schemaManager;
        _queryTranslator = queryTranslator;
        _logger = logger?.ResolveLogger<MaskQueryManager<TLocalQuery, TStartingWith, TSelection, TJoining, TProjection, TMaskSchema>>();
    }

    public abstract Task<Result<LocalData<TDataItem>>> RunQuery<TDataItem>(TLocalQuery query);

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
