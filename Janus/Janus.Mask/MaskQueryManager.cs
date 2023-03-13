using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mask.MaskedDataModel;
using Janus.Mask.MaskedQueryModel;
using Janus.Mask.MaskedSchemaModel;
using Janus.Mask.Translation;

namespace Janus.Mask;
public abstract class MaskQueryManager<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection, TMaskedSchema, TMaskedData, TMaskedDataItem>
    : IDelegatingQueryManager
    where TMaskedQuery : MaskedQuery<TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection>
    where TMaskedSchema : MaskedDataSource
    where TMaskedData : MaskedData<TMaskedDataItem>
{
    private readonly MaskCommunicationNode _communicationNode;
    private readonly MaskSchemaManager<TMaskedSchema> _schemaManager;
    private readonly IMaskQueryTranslator<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection> _queryTranslator;

    private readonly ILogger<MaskQueryManager<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection, TMaskedSchema, TMaskedData, TMaskedDataItem>>? _logger;

    public MaskQueryManager(
        MaskCommunicationNode communicationNode,
        MaskSchemaManager<TMaskedSchema> schemaManager,
        IMaskQueryTranslator<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection> queryTranslator,
        ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _schemaManager = schemaManager;
        _queryTranslator = queryTranslator;
        _logger = logger?.ResolveLogger<MaskQueryManager<TMaskedQuery, TMaskedStartingWith, TMaskedSelection, TMaskedJoining, TMaskedProjection, TMaskedSchema, TMaskedData, TMaskedDataItem>>();
    }

    public abstract Task<Result<TMaskedData>> RunQuery(TMaskedQuery query);

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
