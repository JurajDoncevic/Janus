using FunctionalExtensions.Base;
using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.MediationQueryModels;

/// <summary>
/// Describes a source query joining clause
/// </summary>
public class Joining
{
    private List<Join> _joins;

    /// <summary>
    /// Joins in the clause
    /// </summary>
    public IReadOnlyCollection<Join> Joins => _joins;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="joins">Joins to be added to the clause</param>
    internal Joining(IEnumerable<Join> joins)
    {
        _joins = joins.ToList();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    internal Joining()
    {
        _joins = new List<Join>();
    }

    /// <summary>
    /// Adds a join to the joining clause
    /// </summary>
    /// <param name="join"></param>
    /// <returns></returns>
    internal bool AddJoin(Join join)
    {
        _joins.Add(join);
        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is Joining joining &&
               _joins.SequenceEqual(joining.Joins);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_joins);
    }
}

/// <summary>
/// Describes a join specification
/// </summary>
public class Join
{
    private readonly AttributeId _primaryKeyAttributeId;
    private readonly AttributeId _foreignKeyAttributeId;

    /// <summary>
    /// Referenced tableau id
    /// </summary>
    public TableauId PrimaryKeyTableauId => _primaryKeyAttributeId.ParentTableauId;

    /// <summary>
    /// Referenced attribute id
    /// </summary>
    public AttributeId PrimaryKeyAttributeId => _primaryKeyAttributeId;

    /// <summary>
    /// Referencing tableau id
    /// </summary>
    public TableauId ForeignKeyTableauId => _foreignKeyAttributeId.ParentTableauId;

    /// <summary>
    /// Referencing attribute id
    /// </summary>
    public AttributeId ForeignKeyAttributeId => _foreignKeyAttributeId;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="primaryKeyAttributeId">Referencing attribute id</param>
    /// <param name="foreignKeyAttributeId">Referenced attribute id</param>
    internal Join(AttributeId foreignKeyAttributeId, AttributeId primaryKeyAttributeId)
    {
        if (primaryKeyAttributeId is null)
        {
            throw new ArgumentException($"'{nameof(primaryKeyAttributeId)}' cannot be null or empty.", nameof(primaryKeyAttributeId));
        }

        if (foreignKeyAttributeId is null)
        {
            throw new ArgumentException($"'{nameof(foreignKeyAttributeId)}' cannot be null or empty.", nameof(foreignKeyAttributeId));
        }

        _primaryKeyAttributeId = primaryKeyAttributeId;
        _foreignKeyAttributeId = foreignKeyAttributeId;
    }

    public override bool Equals(object? obj)
    {
        return obj is Join join &&
               _primaryKeyAttributeId.Equals(join._primaryKeyAttributeId) &&
               _foreignKeyAttributeId.Equals(join._foreignKeyAttributeId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_primaryKeyAttributeId, _foreignKeyAttributeId);
    }
}

internal static class JoiningUtils
{
    /// <summary>
    /// Checks if all the primary key tableau references are unique. Each tableau can be used only once in a query.
    /// </summary>
    /// <param name="joining">Current joining clause</param>
    /// <param name="join">New join</param>
    /// <returns><c>true</c> or <c>false</c></returns>
    internal static bool ArePrimaryKeyReferencesUnique(Joining joining, Join join)
    {
        var pkRefs = joining.Joins.Select(j => j.PrimaryKeyAttributeId)
                                  .ToList();
        pkRefs.Add(join.PrimaryKeyAttributeId);

        return pkRefs.Distinct().Count() == pkRefs.Count;
    }

    /// <summary>
    /// Checks if all the primary key tableau references are unique. Each tableau can be used only once in a query.
    /// </summary>
    /// <param name="joining">Current joining clause</param>
    /// <returns><c>true</c> or <c>false</c></returns>
    internal static bool ArePrimaryKeyReferencesUnique(Joining joining)
    {
        var pkRefs = joining.Joins.Select(j => j.PrimaryKeyAttributeId)
                                  .ToList();

        return pkRefs.Distinct().Count() == pkRefs.Count;
    }

    /// <summary>
    /// Checks if the joining creates a connected join graph.
    /// </summary>
    /// <param name="initialTableauId">Query's initial tableau id</param>
    /// <param name="joining">Current joining clause</param>
    /// <returns><c>true</c> or <c>false</c></returns>
    internal static bool IsJoiningConnectedGraph(TableauId initialTableauId, Joining joining)
    {
        // vertices are tableaus
        // joins on tableaus are edges
        List<(TableauId, TableauId)> edges = new(joining.Joins.Map(j => (j.ForeignKeyTableauId, j.PrimaryKeyTableauId)));
        edges = edges.OrderBy(e => e.Item1.ToString()).ToList();

        var getNeighbouringVertices =
            (TableauId vertex) => edges.Where(e => e.Item1.Equals(vertex))
                                  .Select(e => e.Item2)
                                  .ToList();

        Stack<TableauId> verticesToVisit = new Stack<TableauId>();
        HashSet<TableauId> visitedVertices = new HashSet<TableauId>();

        visitedVertices.Add(edges.First().Item1);

        getNeighbouringVertices(edges.First().Item1)
            .ForEach(v => verticesToVisit.Push(v));

        verticesToVisit.Push(initialTableauId);

        while (verticesToVisit.Count > 0)
        {
            var vertex = verticesToVisit.Pop();
            visitedVertices.Add(vertex);

            var neighbouringVertices = getNeighbouringVertices(vertex).ToHashSet();


            neighbouringVertices.Except(visitedVertices)
                                .ToList()
                                .ForEach(v => verticesToVisit.Push(v));

        }

        var allVertices = edges.SelectMany(e => new[] { e.Item1, e.Item2 })
                               .Distinct()
                               .ToHashSet();

        return allVertices.SetEquals(visitedVertices);
    }

    /// <summary>
    /// Checks if the added join creates a cycle
    /// </summary>
    /// <param name="initialTableauId">Query's initial tableau id</param>
    /// <param name="joining">Current joining clause</param>
    /// <param name="join">New join</param>
    /// <returns><c>true</c> or <c>false</c></returns>
    internal static bool IsJoiningCyclic(TableauId initialTableauId, Joining joining, Join join)
    {
        // vertices are tableaus
        // joins on tableaus are edges
        List<(TableauId fk, TableauId pk)> edges = new(joining.Joins.Map(j => (j.ForeignKeyTableauId, j.PrimaryKeyTableauId)));
        edges.Add((join.ForeignKeyTableauId, join.PrimaryKeyTableauId));

        // set the initial tableau vertex first
        // edges = edges.OrderByDescending(e => e.fk.Equals(initialTableauId)).ToList();
        //edges = edges.OrderBy(e => edges.Count(e_ => e_.Item1.Equals(e.Item1)) == 1).ToList();

        var getNeighbouringVertices =
            (TableauId vertex) => edges.Where(e => e.fk.Equals(vertex))
                                  .Select(e => e.pk)
                                  .ToList();

        Stack<TableauId> verticesToVisit = new Stack<TableauId>();
        HashSet<TableauId> visitedVertices = new HashSet<TableauId>();

        // visit the initial tableau vertex
        visitedVertices.Add(initialTableauId);
        // get the initial neighbours and assign them for visists
        getNeighbouringVertices(initialTableauId)
            .ForEach(v => verticesToVisit.Push(v));


        while (verticesToVisit.Count > 0)
        {
            var vertex = verticesToVisit.Pop();
            visitedVertices.Add(vertex);

            var neighbouringVertices = getNeighbouringVertices(vertex);

            if (visitedVertices.Intersect(neighbouringVertices).Count() > 0)
                return true;

            neighbouringVertices.ForEach(v => verticesToVisit.Push(v));
        }



        return false;
    }

    /// <summary>
    /// Checks if the added join creates a cycle
    /// </summary>
    /// <param name="joining">Current joining clause</param>
    /// <returns><c>true</c> or <c>false</c></returns>
    internal static bool IsJoiningCyclic(Joining joining)
    {
        // vertices are tableaus
        // joins on tableaus are edges
        List<(TableauId, TableauId)> edges = new(joining.Joins.Map(j => (j.ForeignKeyTableauId, j.PrimaryKeyTableauId)));
        edges = edges.OrderBy(e => e.Item1).ToList();

        var getNeighbouringVertices =
            (TableauId vertex) => edges.Where(e => e.Item1.Equals(vertex))
                                  .Select(e => e.Item2)
                                  .ToList();

        Stack<TableauId> verticesToVisit = new Stack<TableauId>();
        HashSet<TableauId> visitedVertices = new HashSet<TableauId>();

        visitedVertices.Add(edges.First().Item1);

        getNeighbouringVertices(edges.First().Item1)
            .ForEach(v => verticesToVisit.Push(v));

        while (verticesToVisit.Count > 0)
        {
            var vertex = verticesToVisit.Pop();
            visitedVertices.Add(vertex);

            var neighbouringVertices = getNeighbouringVertices(vertex);

            if (visitedVertices.Intersect(neighbouringVertices).Count() > 0)
                return true;

            neighbouringVertices.ForEach(v => verticesToVisit.Push(v));
        }

        return false;
    }
}
