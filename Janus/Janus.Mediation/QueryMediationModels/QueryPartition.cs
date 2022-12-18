using Janus.Commons.QueryModels;
using Janus.Commons.SchemaModels;
using Janus.Commons.SelectionExpressions;
using static Janus.Commons.SelectionExpressions.Expressions;

namespace Janus.Mediation;

public static partial class QueryModelMediation
{
    internal class QueryPartition
    {
        private readonly TableauId _initialTableauId;
        private readonly List<Join> _joins;
        private readonly HashSet<AttributeId> _projectionAttributeIds;
        private SelectionExpression _selectionExpression;

        public TableauId InitialTableauId => _initialTableauId;
        public IReadOnlyList<Join> Joins => _joins;
        public IReadOnlySet<AttributeId> ProjectionAttributeIds => _projectionAttributeIds;
        public SelectionExpression SelectionExpression { get => _selectionExpression; set => _selectionExpression = value; }

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

        public HashSet<TableauId> ReferencedTableauIds =>
            _joins.Map(j => new List<TableauId> { j.ForeignKeyTableauId, j.PrimaryKeyTableauId })
                 .SelectMany(tblIds => tblIds)
                .Fold(
                    Enumerable.Empty<TableauId>().Append(_initialTableauId),
                    (tableauId, referencedTableauIds) => referencedTableauIds.Append(tableauId)
                ).ToHashSet();

        public bool ContainsTableau(TableauId tableauId)
        {
            return _initialTableauId.Equals(tableauId) ||
                _joins.Any(j => j.ForeignKeyTableauId.Equals(tableauId) || j.PrimaryKeyTableauId.Equals(tableauId));
        }

        public IReadOnlySet<AttributeId> AddProjectionAttributes(IEnumerable<AttributeId> attributeIds)
        {
            _projectionAttributeIds.UnionWith(attributeIds);
            return _projectionAttributeIds;
        }

        public IReadOnlySet<AttributeId> AddProjectionAttribute(AttributeId attributeId)
        {
            _projectionAttributeIds.Add(attributeId);
            return _projectionAttributeIds;
        }

        public IReadOnlyList<Join> AddJoins(IEnumerable<Join> joins)
        {
            _joins.AddRange(joins);
            return _joins;
        }

        public IReadOnlyList<Join> AddJoin(Join join)
        {
            _joins.Add(join);
            return _joins;
        }

        public bool IsReferenced(TableauId tableauId)
            => ReferencedTableauIds.Contains(tableauId);

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
}
