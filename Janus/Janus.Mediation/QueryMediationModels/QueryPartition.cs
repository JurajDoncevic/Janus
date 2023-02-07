using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Mediation.QueryMediationModels;
/// <summary>
/// Partition of a mediated query. Schema model references here are localized.
/// </summary>
internal sealed class QueryPartition
{
    private readonly TableauId _initialTableauId;
    private readonly List<Join> _joins;
    private readonly HashSet<AttributeId> _projectionAttributeIds;
    private SelectionExpression _selectionExpression;

    /// <summary>
    /// Initial tableau id
    /// </summary>
    public TableauId InitialTableauId => _initialTableauId;
    /// <summary>
    /// Joins on this partition
    /// </summary>
    public IReadOnlyList<Join> Joins => _joins;
    /// <summary>
    /// Projection attributes on this partition
    /// </summary>
    public IReadOnlySet<AttributeId> ProjectionAttributeIds => _projectionAttributeIds;
    /// <summary>
    /// Selection on this partition
    /// </summary>
    public SelectionExpression SelectionExpression { get => _selectionExpression; set => _selectionExpression = value; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="initialTableauId">Initial tableau id</param>
    /// <param name="joins">Joins in this partition</param>
    /// <param name="projectionAttributeIds">Projection attributes in this partition</param>
    /// <param name="selectionExpression">Selection on this partition</param>
    internal QueryPartition(TableauId initialTableauId,
                          List<Join>? joins = null,
                          HashSet<AttributeId>? projectionAttributeIds = null,
                          SelectionExpression? selectionExpression = null)
    {
        _initialTableauId = initialTableauId;
        _joins = joins ?? new List<Join>();
        _projectionAttributeIds = projectionAttributeIds ?? new HashSet<AttributeId>();
        _selectionExpression = selectionExpression ?? TRUE();
    }

    /// <summary>
    /// Ids of referenced tableaus in this partition
    /// </summary>
    public HashSet<TableauId> ReferencedTableauIds =>
        _joins.Map(j => new List<TableauId> { j.ForeignKeyTableauId, j.PrimaryKeyTableauId })
             .SelectMany(tblIds => tblIds)
            .Fold(
                Enumerable.Empty<TableauId>().Append(_initialTableauId),
                (tableauId, referencedTableauIds) => referencedTableauIds.Append(tableauId)
            ).ToHashSet();

    /// <summary>
    /// Checks if this partition contains a reference to a tableau with id
    /// </summary>
    /// <param name="tableauId"></param>
    /// <returns></returns>
    public bool ContainsTableau(TableauId tableauId)
    {
        return _initialTableauId.Equals(tableauId) ||
            _joins.Any(j => j.ForeignKeyTableauId.Equals(tableauId) || j.PrimaryKeyTableauId.Equals(tableauId));
    }

    /// <summary>
    /// Adds projection attributes in the projection
    /// </summary>
    /// <param name="attributeIds"></param>
    /// <returns>Set of all current projection attributes</returns>
    public IReadOnlySet<AttributeId> AddProjectionAttributes(IEnumerable<AttributeId> attributeIds)
    {
        _projectionAttributeIds.UnionWith(attributeIds);
        return _projectionAttributeIds;
    }

    /// <summary>
    /// Adds a projection attribute
    /// </summary>
    /// <param name="attributeId"></param>
    /// <returns>Set of all current projection attributes</returns>
    public IReadOnlySet<AttributeId> AddProjectionAttribute(AttributeId attributeId)
    {
        _projectionAttributeIds.Add(attributeId);
        return _projectionAttributeIds;
    }

    /// <summary>
    /// Adds joins to the partitioned query
    /// </summary>
    /// <param name="joins"></param>
    /// <returns>All current joins</returns>
    public IReadOnlyList<Join> AddJoins(IEnumerable<Join> joins)
    {
        _joins.AddRange(joins);
        return _joins;
    }

    /// <summary>
    /// Adds a join to the partitioned query
    /// </summary>
    /// <param name="join"></param>
    /// <returns>All current joins</returns>
    public IReadOnlyList<Join> AddJoin(Join join)
    {
        _joins.Add(join);
        return _joins;
    }

    /// <summary>
    /// Checks if the tableau with given id is referenced in this partition
    /// </summary>
    /// <param name="tableauId"></param>
    /// <returns></returns>
    public bool IsReferenced(TableauId tableauId)
        => ReferencedTableauIds.Contains(tableauId);

    /// <summary>
    /// Checks if the parent tableau if the attribute with given id is referenced in this partition
    /// </summary>
    /// <param name="attributeId"></param>
    /// <returns></returns>
    public bool IsParentTableauReferenced(AttributeId attributeId)
        => ReferencedTableauIds.Any(tblId => tblId.IsParentOf(attributeId));

    public override bool Equals(object? obj)
    {
        return obj is QueryPartition other &&
               InitialTableauId == other.InitialTableauId &&
               Joins.SequenceEqual(other.Joins);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(InitialTableauId, Joins);
    }
}

