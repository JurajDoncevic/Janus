using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels;

public class QueryBuilder
{
    private Option<Projection> _projection;
    private Option<Selection> _selection;
    private Option<Joining> _joining;
    private readonly string _queryOnTableauId;
    private readonly DataSource _dataSource;
    private HashSet<string> _referencedTableaus;

    private QueryBuilder(string queryOnTableauId!!, DataSource dataSource!!)
    {
        _projection = Option<Projection>.None;
        _selection = Option<Selection>.None;
        _joining = Option<Joining>.None;
        _queryOnTableauId = queryOnTableauId;
        _dataSource = dataSource;
        _referencedTableaus = new HashSet<string>();
        _referencedTableaus.Add(queryOnTableauId);
    }

    public static QueryBuilder InitQueryOnTableau(string tableauId, DataSource dataSource)
    {
        if(!dataSource.ContainsTableau(tableauId))
            throw new TableauDoesNotExistException(tableauId, dataSource.Name);

        return new QueryBuilder(tableauId, dataSource);
    }

    public QueryBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration)
    {
        var builder = new SelectionBuilder(_dataSource, _referencedTableaus);
        builder = configuration(builder);
        _selection = Option<Selection>.Some(builder.Build());
        return this;
    }

    public QueryBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration)
    {
        var builder = new ProjectionBuilder(_dataSource, _referencedTableaus);
        builder = configuration(builder);
        _projection = Option<Projection>.Some(builder.Build());
        return this;
    }

    public QueryBuilder WithJoining(Func<JoiningBuilder, JoiningBuilder> configuration)
    {
        var builder = new JoiningBuilder(_dataSource);
        builder = configuration(builder);
        _joining = Option<Joining>.Some(builder.Build());
        return this;
    }

    public Query Build()
    {
        return new Query(_queryOnTableauId, _projection, _selection, _joining);
    }

}

public class SelectionBuilder
{
    private string _expression;
    private readonly DataSource _dataSource;
    private readonly HashSet<string> _referencedTableaus;

    internal SelectionBuilder(DataSource dataSource,  HashSet<string> referencedTableaus)
    {
        _expression = "";
        _dataSource = dataSource;
        _referencedTableaus = referencedTableaus;
    }

    public SelectionBuilder WithExpression(string expression)
    {
        // TODO: do checks
        _expression = expression;
        return this;
    }

    internal Selection Build()
    {
        return new Selection(_expression);
    }
}

public class ProjectionBuilder
{
    private readonly DataSource _dataSource;
    private HashSet<string> _projectionAttributes;
    private readonly HashSet<string> _referencedTableaus;

    internal ProjectionBuilder(DataSource dataSource!!, HashSet<string> referencedTableaus!!)
    {
        _dataSource = dataSource;
        _projectionAttributes = new HashSet<string>();
        _referencedTableaus = referencedTableaus;
    }

    public ProjectionBuilder AddAttribute(string attributeId)
    {
        if (!_dataSource.ContainsAttribute(attributeId))
            throw new AttributeDoesNotExistException(attributeId, _dataSource.Name);
        if(!_dataSource.Schemas
                       .SelectMany(schema => schema.Tableaus)
                       .Where(tableau => _referencedTableaus.Contains(tableau.Id))
                       .Any(tableau => tableau.Attributes.Any(attribute => attribute.Id.Equals(attributeId))))
            throw new AttributeNotInReferencedTableausException(attributeId);
        if (_projectionAttributes.Contains(attributeId))
            throw new DuplicateAttributeAssignedToProjectionException(attributeId);

        _projectionAttributes.Add(attributeId);

        return this;
    }

    internal Projection Build()
    {
        return new Projection(_projectionAttributes);
    }
}

public class JoiningBuilder
{
    private readonly DataSource _dataSource;
    private Joining _joining;
    // TODO: check references, check circular joins
    internal JoiningBuilder(DataSource dataSource!!)
    {
        _dataSource = dataSource;
        _joining = new Joining();
    }

    public JoiningBuilder AddJoin(string foreignKeyAttributeId, string primaryKeyAttributeId)
    {
        // check if given ids are ok
        if (!foreignKeyAttributeId.Contains('.'))
            throw new InvalidAttributeIdException(foreignKeyAttributeId);
        if (!primaryKeyAttributeId.Contains('.'))
            throw new InvalidAttributeIdException(primaryKeyAttributeId);

        //get tableau ids from the supposed attribute ids
        var foreignKeyTableauId = foreignKeyAttributeId.Remove(foreignKeyAttributeId.LastIndexOf('.')); 
        var primaryKeyTableauId = primaryKeyAttributeId.Remove(primaryKeyAttributeId.LastIndexOf('.'));

        if (!_dataSource.ContainsAttribute(foreignKeyAttributeId))
            throw new AttributeDoesNotExistException(primaryKeyAttributeId, _dataSource.Name);
        if(!_dataSource.ContainsAttribute(primaryKeyAttributeId))
            throw new AttributeDoesNotExistException(primaryKeyAttributeId, _dataSource.Name);
        if(foreignKeyTableauId.Equals(primaryKeyTableauId))
            throw new SelfJoinNotSupportedException(primaryKeyTableauId);

        Join join = new Join(primaryKeyTableauId, primaryKeyAttributeId, foreignKeyTableauId, foreignKeyAttributeId);

        if(!IsConnectedGraph(_joining, join)) // maybe move to build?
            throw new JoinsNotConnectedException();
        if(DoesCreateCycle(_joining, join))
            throw new CyclicJoinNotSupportedException(join);
        if (!ArePrimaryKeyReferencesUnique(_joining, join))
            throw new TableauRereferencedAsPrimaryException(join);
        if (_joining.Joins.Contains(join))
            throw new DuplicateJoinNotSupportedException(join);
        _joining.AddJoin(join);

        return this;
    }

    private bool ArePrimaryKeyReferencesUnique(Joining joining, Join join)
    {
        var pkRefs = joining.Joins.Select(j => j.PrimaryKeyAttributeId)
                                  .ToList();
        pkRefs.Add(join.PrimaryKeyAttributeId);

        return pkRefs.Distinct().Count() == pkRefs.Count;
    }

    private bool IsConnectedGraph(Joining joining, Join join)
    {
        // vertices are tableaus
        // joins on tableaus are edges
        List<(string, string)> edges = new(joining.Joins.Map(j => (j.ForeignKeyTableauId, j.PrimaryKeyTableauId)));
        edges.Add((join.ForeignKeyTableauId, join.PrimaryKeyTableauId));
        edges = edges.OrderBy(e => e.Item1).ToList();

        var getNeighbouringVertices =
            (string edge) => edges.Where(e => e.Item1.Equals(edge))
                                  .Select(e => e.Item2)
                                  .ToList();

        Stack<string> verticesToVisit = new Stack<string>();
        HashSet<string> visitedVertices = new HashSet<string>();

        visitedVertices.Add(edges.First().Item1);

        getNeighbouringVertices(edges.First().Item1)
            .ForEach(v => verticesToVisit.Push(v));

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

    private bool DoesCreateCycle(Joining joining, Join join)
    {
        // vertices are tableaus
        // joins on tableaus are edges
        List<(string, string)> edges = new(joining.Joins.Map(j => (j.ForeignKeyTableauId, j.PrimaryKeyTableauId)));
        edges.Add((join.ForeignKeyTableauId, join.PrimaryKeyTableauId));
        edges = edges.OrderBy(e => e.Item1).ToList();

        var getNeighbouringVertices =
            (string edge) => edges.Where(e => e.Item1.Equals(edge))
                                  .Select(e => e.Item2)
                                  .ToList();

        Stack<string> verticesToVisit = new Stack<string>();
        HashSet<string> visitedVertices = new HashSet<string>();

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

    public Joining Build()
    {
        return _joining;
    }


}


