using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediation.QueryMediationModels;

/// <summary>
/// Describes a mediation for a query over a mediated data source
/// </summary>
public sealed class QueryMediation
{
    private readonly Dictionary<TableauId, Query> _partitionedQueries;
    private readonly List<Join> _finalizingJoins;
    private readonly Option<SelectionExpression> _finalizingSelection;
    private readonly HashSet<AttributeId> _finalizingProjection;
    private readonly Query _originalQuery;
    private readonly DataSourceMediation _dataSourceMediation;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="originalQuery">Original query on the mediated schema</param>
    /// <param name="partitionedQueries">Partitioned queries</param>
    /// <param name="finalizingJoins">Finalizing joins- joins to be executed on the query mediating component</param>
    /// <param name="finalizingSelection">Finalizing selection - selection to be executed on the query mediating component</param>
    /// <param name="finalizingProjection">Finalizing projection - projection to be executed on the query mediating component</param>
    /// <param name="dataSourceMediation">Data source mediation used to create this query mediation</param>
    public QueryMediation(Query originalQuery,
                          IEnumerable<Query> partitionedQueries,
                          IEnumerable<Join> finalizingJoins,
                          Option<SelectionExpression> finalizingSelection,
                          IEnumerable<AttributeId> finalizingProjection,
                          DataSourceMediation dataSourceMediation)
    {
        _originalQuery = originalQuery;
        _partitionedQueries = partitionedQueries.ToDictionary(pq => pq.OnTableauId, pq => pq);
        _finalizingJoins = finalizingJoins.ToList();
        _finalizingSelection = finalizingSelection;
        _finalizingProjection = finalizingProjection.ToHashSet();
        _dataSourceMediation = dataSourceMediation;
    }

    /// <summary>
    /// Target tableau ids and their partitioned queries
    /// </summary>
    public IReadOnlyDictionary<TableauId, Query> PartitionedQueries => _partitionedQueries;

    /// <summary>
    /// All partitioned queries
    /// </summary>
    public IReadOnlyList<Query> PartitionedQueryList => _partitionedQueries.Values.ToList();

    /// <summary>
    /// Finalizing joins - joins to be executed on the query mediating component
    /// </summary>
    public IReadOnlyList<Join> FinalizingJoins => _finalizingJoins;

    /// <summary>
    /// Finalizing selection - selection to be executed on the query mediating component
    /// </summary>
    public Option<SelectionExpression> FinalizingSelection => _finalizingSelection;

    /// <summary>
    /// Finalizing projection - projection to be executed on the query mediating component
    /// </summary>
    public IReadOnlySet<AttributeId> FinalizingProjection => _finalizingProjection;

    /// <summary>
    /// Original query over a mediated schema
    /// </summary>
    public Query OriginalQuery => _originalQuery;

    /// <summary>
    /// Schema mediation used to create this query mediation
    /// </summary>
    public DataSourceMediation DataSourceMediation => _dataSourceMediation;
}
