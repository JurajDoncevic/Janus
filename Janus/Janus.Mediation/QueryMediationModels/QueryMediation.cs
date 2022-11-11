using Janus.Commons.QueryModels;
using Janus.Commons.SelectionExpressions;

namespace Janus.Mediation.QueryMediationModels;
public class QueryMediation
{
    private readonly Dictionary<string, Query> _queriesForDataSources;
    private readonly List<MediationJoin> _finalizingJoins;
    private readonly SelectionExpression _finalizingSelection;
    private readonly MediationProjection _finalizingProjection;

    public QueryMediation(Dictionary<string, Query> queriesForDataSources, List<MediationJoin> finalizingJoins, SelectionExpression finalizingSelection, MediationProjection finalizingProjection)
    {
        _queriesForDataSources = queriesForDataSources;
        _finalizingJoins = finalizingJoins;
        _finalizingSelection = finalizingSelection;
        _finalizingProjection = finalizingProjection;
    }

    public IReadOnlyDictionary<string, Query> QueriesForDataSources => _queriesForDataSources;

    public IReadOnlyList<MediationJoin> FinalizingJoins => _finalizingJoins;

    public SelectionExpression FinalizingSelection => _finalizingSelection;

    public MediationProjection FinalizingProjection => _finalizingProjection;
}
