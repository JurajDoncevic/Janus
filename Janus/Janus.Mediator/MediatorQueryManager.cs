using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Components;
using Janus.Logging;
using Janus.Mediation;

namespace Janus.Mediator;

/// <summary>
/// Query manager for a mediator component
/// </summary>
public sealed class MediatorQueryManager : IDelegatingQueryManager, IMediatingQueryManager<MediatorSchemaManager>
{
    private readonly MediatorCommunicationNode _communicationNode;
    private readonly ILogger<MediatorQueryManager>? _logger;

    public MediatorQueryManager(MediatorCommunicationNode communicationNode, ILogger? logger = null)
    {
        _communicationNode = communicationNode;
        _logger = logger?.ResolveLogger<MediatorQueryManager>();
    }

    /// <summary>
    /// Runs a mediated query over other remote nodes
    /// </summary>
    /// <param name="query">Query on the mediated data source</param>
    /// <param name="schemaManager">Current schema manager</param>
    /// <returns>Tabular data query result</returns>
    public async Task<Result<TabularData>> RunQuery(Query query, MediatorSchemaManager schemaManager)
        => (await Results.AsResult(async () =>
        {
            var currentMediatedSchema = schemaManager.CurrentMediatedSchema;
            if (!currentMediatedSchema)
            {
                return Results.OnFailure<TabularData>($"No mediated schema generated");
            }

            var currentMediation = schemaManager.CurrentMediation;
            if (!currentMediation)
            {
                return Results.OnFailure<TabularData>($"No mediation created");
            }

            var queryValidation = query.IsValidForDataSource(currentMediatedSchema.Value);
            if (!queryValidation)
            {
                return Results.OnFailure<TabularData>($"Query {query.Name} is not valid on mediated schema {currentMediatedSchema.Value.Name}: {queryValidation.Message}");
            }

            // mediate the query into query partitions
            var queryMediationResult = QueryModelMediation.MediateQuery(currentMediatedSchema.Value, currentMediation.Value, query);
            if (!queryMediationResult)
            {
                return Results.OnFailure<TabularData>($"Failed query {query.Name} mediation with message: {queryMediationResult.Message}");
            }
            var queryMediation = queryMediationResult.Data;
            
            // start to run remote queries in parallel
            var remoteQueryTasks = new List<Task<Result<TabularData>>>();
            foreach (var partitionedQuery in queryMediation.PartitionedQueries)
            {
                var targetRemotePoint = schemaManager.RemotePointWithLoadedDataSourceName[partitionedQuery.Key.DataSourceName];

                var remoteQueryTask = _communicationNode.SendQueryRequest(partitionedQuery.Value, targetRemotePoint);

                remoteQueryTasks.Add(remoteQueryTask);
            }

            // await all query results
            var remoteQueryResults = await Task.WhenAll(remoteQueryTasks);
            if(!remoteQueryResults.All(_ => _))
            {
                var failedQueryResults = remoteQueryResults.Where(qr => !qr.IsSuccess);
                return Results.OnFailure<TabularData>($"Failed query runs with: {string.Join(", ", failedQueryResults.Select(qr => qr.Message))}");
            }

            // get the remote query execution results' tabular data 
            var resultsTabularData = remoteQueryResults.Select(qr => qr.Data);
            var resultMediation = QueryModelMediation.MediateQueryResults(queryMediation, resultsTabularData);
            if (!resultMediation)
            {
                return Results.OnFailure<TabularData>($"Failed query result mediation with message: {resultMediation.Message}");
            }

            return resultMediation;
        })).Pass(
                r => _logger?.Info($"Successful query {query.Name} run"),
                r => _logger?.Info($"Failed query {query.Name} run with message: {r.Message}")
            );

    public async Task<Result<TabularData>> RunQueryOn(Query query, RemotePoint remotePoint)
        => (await Results.AsResult(async () =>
        {
            var queryReqResult = await _communicationNode.SendQueryRequest(query, remotePoint);

            return queryReqResult;
        })).Pass(
                r => _logger?.Info($"Successful query {query.Name} run on {remotePoint}"),
                r => _logger?.Info($"Failed query {query.Name} run on {remotePoint}")
            );
}
