using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using Janus.Mediation.SchemaMediationModels;

namespace Janus.Mediation.QueryMediationModels;
public class QueryMediation
{
    private readonly Dictionary<TableauId, Query> _partitionedQueries;
    private readonly List<Join> _finalizingJoins;
    private readonly Option<SelectionExpression> _finalizingSelection;
    private readonly HashSet<AttributeId> _finalizingProjection;
    private readonly Query _originalQuery;
    private readonly DataSourceMediation _dataSourceMediation;

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

    public IReadOnlyDictionary<TableauId, Query> PartitionedQueries => _partitionedQueries;

    public IReadOnlyList<Query> PartitionedQueryList => _partitionedQueries.Values.ToList();

    public IReadOnlyList<Join> FinalizingJoins => _finalizingJoins;

    public Option<SelectionExpression> FinalizingSelection => _finalizingSelection;

    public IReadOnlySet<AttributeId> FinalizingProjection => _finalizingProjection;

    public Query OriginalQuery => _originalQuery;

    public DataSourceMediation DataSourceMediation => _dataSourceMediation;
}
