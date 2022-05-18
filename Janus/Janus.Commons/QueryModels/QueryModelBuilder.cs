using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels;

#region BUILDER SEQUENCE INTERFACES
public interface IPostProjectionBuilder
{
    public IPostSelectionBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration);
    public Query Build();
}

public interface IPostSelectionBuilder
{
    public IPostProjectionBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration);
    public Query Build();
}

public interface IPostJoiningBuilder
{
    public IPostProjectionBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration);
    public IPostSelectionBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration);
    public Query Build();
}

public interface IPostInitBuilder
{
    public IPostJoiningBuilder WithJoining(Func<JoiningBuilder, JoiningBuilder> configuration);
    public Query Build();
}
#endregion

/// <summary>
/// Query model builer class used to create a query
/// </summary>
public class QueryModelBuilder : IPostInitBuilder, IPostJoiningBuilder, IPostSelectionBuilder, IPostProjectionBuilder
{
    private Option<Projection> _projection;
    private Option<Selection> _selection;
    private Option<Joining> _joining;
    private readonly string _queryOnTableauId;
    private readonly DataSource _dataSource;
    private HashSet<string> _referencedTableaus;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="queryOnTableauId">Id of tableau on which the query is initialized</param>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    private QueryModelBuilder(string queryOnTableauId!!, DataSource dataSource!!)
    {
        _projection = Option<Projection>.None;
        _selection = Option<Selection>.None;
        _joining = Option<Joining>.None;
        _queryOnTableauId = queryOnTableauId;
        _dataSource = dataSource;
        _referencedTableaus = new HashSet<string>();
        _referencedTableaus.Add(queryOnTableauId);
    }

    /// <summary>
    /// Initializes the query on a tableau found in the given data source
    /// </summary>
    /// <param name="tableauId">Id of tableau on which the query is initialized</param>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <returns>QueryModelBuilder</returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static QueryModelBuilder InitQueryOnTableau(string tableauId!!, DataSource dataSource!!)
    {
        if(!dataSource.ContainsTableau(tableauId))
            throw new TableauDoesNotExistException(tableauId, dataSource.Name);

        return new QueryModelBuilder(tableauId, dataSource);
    }

    /// <summary>
    /// Specifies a selection clause of the query
    /// </summary>
    /// <param name="configuration">Selection configuration over a <see cref="SelectionBuilder"/></param>
    /// <returns>QueryModelBuilder</returns>
    public IPostSelectionBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration)
    {
        var builder = new SelectionBuilder(_dataSource, _referencedTableaus);
        builder = configuration(builder);
        _selection = Option<Selection>.Some(builder.Build());
        return this;
    }
    /// <summary>
    /// Specifies a projection clause of the query 
    /// </summary>
    /// <param name="configuration">Projection configuration over a <see cref="ProjectionBuilder"/></param>
    /// <returns>QueryModelBuilder</returns>
    public IPostProjectionBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration)
    {
        var builder = new ProjectionBuilder(_dataSource, _referencedTableaus);
        builder = configuration(builder);
        _projection = Option<Projection>.Some(builder.Build());
        return this;
    }

    /// <summary>
    /// Specifies a joining clause of the query
    /// </summary>
    /// <param name="configuration">Joining configuration over a <see cref="JoiningBuilder"/></param>
    /// <returns>QueryModelBuilder</returns>
    public IPostJoiningBuilder WithJoining(Func<JoiningBuilder, JoiningBuilder> configuration)
    {
        var builder = new JoiningBuilder(_dataSource);
        builder = configuration(builder);
        _joining = Option<Joining>.Some(builder.Build());
        return this;
    }

    /// <summary>
    /// Builds the specified query
    /// </summary>
    /// <returns>Query</returns>
    public Query Build()
    {
        return new Query(_queryOnTableauId, _projection, _selection, _joining);
    }
}

/// <summary>
/// Builder class for query selection
/// </summary>
public class SelectionBuilder
{
    private string _expression;
    private readonly DataSource _dataSource;
    private readonly HashSet<string> _referencedTableaus;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableaus">Ids of tableaus referenced in the query</param>
    internal SelectionBuilder(DataSource dataSource!!,  HashSet<string> referencedTableaus!!)
    {
        _expression = "";
        _dataSource = dataSource;
        _referencedTableaus = referencedTableaus;
    }

    /// <summary>
    /// Creates the selection expression
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>SelectionBuilder</returns>
    public SelectionBuilder WithExpression(string expression)
    {
        // TODO: do checks
        _expression = expression;
        return this;
    }

    /// <summary>
    /// Builds the specified selection
    /// </summary>
    /// <returns></returns>
    internal Selection Build()
    {
        return new Selection(_expression);
    }
}

/// <summary>
/// Builder class for query projection
/// </summary>
public class ProjectionBuilder
{
    private readonly DataSource _dataSource;
    private HashSet<string> _projectionAttributes;
    private readonly HashSet<string> _referencedTableaus;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableaus">Ids of tableaus referenced in the query</param>
    internal ProjectionBuilder(DataSource dataSource!!, HashSet<string> referencedTableaus!!)
    {
        _dataSource = dataSource;
        _projectionAttributes = new HashSet<string>();
        _referencedTableaus = referencedTableaus;
    }

    /// <summary>
    /// Adds an attribute to the projection
    /// </summary>
    /// <param name="attributeId">Attribute id</param>
    /// <returns>ProjectionBuilder</returns>
    /// <exception cref="AttributeDoesNotExistException"></exception>
    /// <exception cref="AttributeNotInReferencedTableausException"></exception>
    /// <exception cref="DuplicateAttributeAssignedToProjectionException"></exception>
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

    /// <summary>
    /// Builds the specified projection
    /// </summary>
    /// <returns>Projection</returns>
    internal Projection Build()
    {
        return new Projection(_projectionAttributes);
    }
}

/// <summary>
/// Builder class for query joins
/// </summary>
public class JoiningBuilder
{
    private readonly DataSource _dataSource;
    private Joining _joining;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    internal JoiningBuilder(DataSource dataSource!!)
    {
        _dataSource = dataSource;
        _joining = new Joining();
    }

    /// <summary>
    /// Adds a join to the joining clause
    /// </summary>
    /// <param name="foreignKeyAttributeId">Attribute id of the foreign key</param>
    /// <param name="primaryKeyAttributeId">Attribute id of the primary key</param>
    /// <returns></returns>
    /// <exception cref="InvalidAttributeIdException"></exception>
    /// <exception cref="AttributeDoesNotExistException"></exception>
    /// <exception cref="SelfJoinNotSupportedException"></exception>
    /// <exception cref="CyclicJoinNotSupportedException"></exception>
    /// <exception cref="TableauPrimaryKeyReferenceNotUniqueException"></exception>
    /// <exception cref="DuplicateJoinNotSupportedException"></exception>
    public JoiningBuilder AddJoin(string foreignKeyAttributeId, string primaryKeyAttributeId)
    {
        // check if given ids are ok
        if (!foreignKeyAttributeId.Contains('.'))
            throw new InvalidAttributeIdException(foreignKeyAttributeId);
        if (!primaryKeyAttributeId.Contains('.'))
            throw new InvalidAttributeIdException(primaryKeyAttributeId);

        // get tableau ids from the supposed attribute ids
        var foreignKeyTableauId = foreignKeyAttributeId.Remove(foreignKeyAttributeId.LastIndexOf('.')); 
        var primaryKeyTableauId = primaryKeyAttributeId.Remove(primaryKeyAttributeId.LastIndexOf('.'));

        // check attribute (and tableau) existence
        if (!_dataSource.ContainsAttribute(foreignKeyAttributeId))
            throw new AttributeDoesNotExistException(primaryKeyAttributeId, _dataSource.Name);
        if(!_dataSource.ContainsAttribute(primaryKeyAttributeId))
            throw new AttributeDoesNotExistException(primaryKeyAttributeId, _dataSource.Name);

        // check for self-join
        if(foreignKeyTableauId.Equals(primaryKeyTableauId))
            throw new SelfJoinNotSupportedException(primaryKeyTableauId);

        Join join = new Join(primaryKeyTableauId, primaryKeyAttributeId, foreignKeyTableauId, foreignKeyAttributeId);

        // check for cycle joins, pk tableaus multiple references, duplicate joins
        if (_joining.Joins.Contains(join))
            throw new DuplicateJoinNotSupportedException(join);
        if (IsJoiningCyclic(_joining, join))
            throw new CyclicJoinNotSupportedException(join);
        if (!ArePrimaryKeyReferencesUnique(_joining, join))
            throw new TableauPrimaryKeyReferenceNotUniqueException(join);
        _joining.AddJoin(join);

        return this;
    }

    /// <summary>
    /// Checks if all the primary key tableau references are unique. Each table can be used only once in a query.
    /// </summary>
    /// <param name="joining">Current joining clause</param>
    /// <param name="join">New join</param>
    /// <returns><c>true</c> or <c>false</c></returns>
    private bool ArePrimaryKeyReferencesUnique(Joining joining, Join join)
    {
        var pkRefs = joining.Joins.Select(j => j.PrimaryKeyAttributeId)
                                  .ToList();
        pkRefs.Add(join.PrimaryKeyAttributeId);

        return pkRefs.Distinct().Count() == pkRefs.Count;
    }

    /// <summary>
    /// Checks if the joining creates a connected join graph.
    /// </summary>
    /// <param name="joining">Current joining clause</param>
    /// <returns><c>true</c> or <c>false</c></returns>
    private bool IsJoiningConnectedGraph(Joining joining)
    {
        // vertices are tableaus
        // joins on tableaus are edges
        List<(string, string)> edges = new(joining.Joins.Map(j => (j.ForeignKeyTableauId, j.PrimaryKeyTableauId)));
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

    /// <summary>
    /// Checks if the added join creates a cycle
    /// </summary>
    /// <param name="joining">Current joining clause</param>
    /// <param name="join">New join</param>
    /// <returns><c>true</c> or <c>false</c></returns>
    private bool IsJoiningCyclic(Joining joining, Join join)
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

    /// <summary>
    /// Builds the joining clause
    /// </summary>
    /// <returns>Joining</returns>
    /// <exception cref="JoinsNotConnectedException"></exception>
    public Joining Build()
    {
        if (!IsJoiningConnectedGraph(_joining)) // maybe move to build?
            throw new JoinsNotConnectedException();
        return _joining;
    }


}


