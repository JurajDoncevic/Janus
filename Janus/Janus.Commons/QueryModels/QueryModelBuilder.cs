using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels;

#region BUILDER SEQUENCE INTERFACES

/// <summary>
/// Determines what declarations can be made after a projection declaration
/// </summary>
public interface IPostProjectionBuilder
{
    /// <summary>
    /// Specifies a selection clause of the query
    /// </summary>
    /// <param name="configuration">Selection configuration over a <see cref="SelectionBuilder"/></param>
    /// <returns>IPostSelectionBuilder</returns>
    public IPostSelectionBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration);

    /// <summary>
    /// Builds the specified query
    /// </summary>
    /// <returns>Query</returns>
    public Query Build();
}

/// <summary>
/// Determines what declarations can be made after a selection declaration
/// </summary>
public interface IPostSelectionBuilder
{
    /// <summary>
    /// Specifies a projection clause of the query 
    /// </summary>
    /// <param name="configuration">Projection configuration over a <see cref="ProjectionBuilder"/></param>
    /// <returns>IPostProjectionBuilder</returns>
    public IPostProjectionBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration);

    /// <summary>
    /// Builds the specified query
    /// </summary>
    /// <returns>Query</returns>
    public Query Build();
}

/// <summary>
/// Determines what declarations can be made after a joining declaration
/// </summary>
public interface IPostJoiningBuilder
{
    /// <summary>
    /// Specifies a projection clause of the query 
    /// </summary>
    /// <param name="configuration">Projection configuration over a <see cref="ProjectionBuilder"/></param>
    /// <returns>IPostProjectionBuilder</returns>
    public IPostProjectionBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration);

    /// <summary>
    /// Specifies a selection clause of the query
    /// </summary>
    /// <param name="configuration">Selection configuration over a <see cref="SelectionBuilder"/></param>
    /// <returns>IPostSelectionBuilder</returns>
    public IPostSelectionBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration);

    /// <summary>
    /// Builds the specified query
    /// </summary>
    /// <returns>Query</returns>
    public Query Build();
}

/// <summary>
/// Determines what declarations can be made after a query initialization
/// </summary>
public interface IPostInitBuilder
{
    /// <summary>
    /// Specifies a joining clause of the query
    /// </summary>
    /// <param name="configuration">Joining configuration over a <see cref="JoiningBuilder"/></param>
    /// <returns>IPostJoiningBuilder</returns>
    public IPostJoiningBuilder WithJoining(Func<JoiningBuilder, JoiningBuilder> configuration);

    /// <summary>
    /// Specifies a projection clause of the query 
    /// </summary>
    /// <param name="configuration">Projection configuration over a <see cref="ProjectionBuilder"/></param>
    /// <returns>IPostProjectionBuilder</returns>
    public IPostProjectionBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration);

    /// <summary>
    /// Specifies a selection clause of the query
    /// </summary>
    /// <param name="configuration">Selection configuration over a <see cref="SelectionBuilder"/></param>
    /// <returns>IPostSelectionBuilder</returns>
    public IPostSelectionBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration);

    /// <summary>
    /// Builds the specified query
    /// </summary>
    /// <returns>Query</returns>
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
    private HashSet<string> _referencedTableauIds;

    /// <summary>
    /// Constructor. Used when build-time validation is required.
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
        _referencedTableauIds = new HashSet<string>();
        _referencedTableauIds.Add(queryOnTableauId);
    }


    /// <summary>
    /// Initializes the query on a tableau found in the given data source. Generated query is valid on the data source.
    /// </summary>
    /// <param name="onTableauId">Id of tableau on which the query is initialized</param>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <returns>QueryModelBuilder</returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitBuilder InitQueryOnDataSource(string onTableauId!!, DataSource dataSource!!)
    {
        if(!dataSource.ContainsTableau(onTableauId))
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);

        return new QueryModelBuilder(onTableauId, dataSource);
    }

    /// <summary>
    /// Specifies a selection clause of the query
    /// </summary>
    /// <param name="configuration">Selection configuration over a <see cref="SelectionBuilder"/></param>
    /// <returns>QueryModelBuilder</returns>
    public IPostSelectionBuilder WithSelection(Func<SelectionBuilder, SelectionBuilder> configuration)
    {
        var builder = new SelectionBuilder(_dataSource, _referencedTableauIds);
        builder = configuration(builder);
        
        _selection = builder.IsConfigured
                     ? Option<Selection>.Some(builder.Build())
                     : Option<Selection>.None;
        return this;
    }
    /// <summary>
    /// Specifies a projection clause of the query 
    /// </summary>
    /// <param name="configuration">Projection configuration over a <see cref="ProjectionBuilder"/></param>
    /// <returns>QueryModelBuilder</returns>
    public IPostProjectionBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration)
    {
        var builder = new ProjectionBuilder(_dataSource, _referencedTableauIds);
        builder = configuration(builder);
        _projection = builder.IsConfigured
                     ? Option<Projection>.Some(builder.Build())  
                     : Option<Projection>.None;
        return this;
    }

    /// <summary>
    /// Specifies a joining clause of the query
    /// </summary>
    /// <param name="configuration">Joining configuration over a <see cref="JoiningBuilder"/></param>
    /// <returns>QueryModelBuilder</returns>
    public IPostJoiningBuilder WithJoining(Func<JoiningBuilder, JoiningBuilder> configuration)
    {
        var builder = new JoiningBuilder(_queryOnTableauId, _dataSource);
        builder = configuration(builder);
        _joining = builder.IsConfigured
                   ? Option<Joining>.Some(builder.Build())
                   : Option<Joining>.None;

        if (_joining)
        {
            _joining.Value.Joins.SelectMany(j => new[] { j.ForeignKeyTableauId, j.PrimaryKeyTableauId })
                    .ToList()
                    .ForEach(tableauId => _referencedTableauIds.Add(tableauId));
        }

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
    private readonly HashSet<string> _referencedTableauIds;
    internal bool IsConfigured => !string.IsNullOrEmpty(_expression);

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableauIds">Ids of tableaus referenced in the query</param>
    internal SelectionBuilder(DataSource dataSource!!, HashSet<string> referencedTableauIds!!)
    {
        _expression = "";
        _dataSource = dataSource;
        _referencedTableauIds = referencedTableauIds;
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
    private readonly HashSet<string> _referencedTableauIds;
    internal bool IsConfigured => _projectionAttributes.Count > 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableauIds">Ids of tableaus referenced in the query</param>
    internal ProjectionBuilder(DataSource dataSource!!, HashSet<string> referencedTableauIds!!)
    {
        _dataSource = dataSource;
        _projectionAttributes = new HashSet<string>();
        _referencedTableauIds = referencedTableauIds;
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
        if (!_dataSource.Schemas
                       .SelectMany(schema => schema.Tableaus)
                       .Where(tableau => _referencedTableauIds.Contains(tableau.Id))
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
    private readonly string _initialTableauId;
    private readonly DataSource _dataSource;
    private Joining _joining;

    internal bool IsConfigured => _joining.Joins.Count > 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    internal JoiningBuilder(string initialTableauId, DataSource dataSource!!)
    {
        _initialTableauId = initialTableauId;
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
        if (!_dataSource.ContainsAttribute(primaryKeyAttributeId))
            throw new AttributeDoesNotExistException(primaryKeyAttributeId, _dataSource.Name);

        // check attribute types - must be the same
        var fkNames = Utils.GetNamesFromAttributeId(foreignKeyAttributeId);
        var pkNames = Utils.GetNamesFromAttributeId(primaryKeyAttributeId);
        var fkAttribute = _dataSource[fkNames.schemaName][fkNames.tableauName][fkNames.attributeName];
        var pkAttribute = _dataSource[pkNames.schemaName][pkNames.tableauName][pkNames.attributeName];
        if (fkAttribute.DataType != pkAttribute.DataType)
            throw new JoinedAttributesNotOfSameTypeException(foreignKeyAttributeId, fkAttribute.DataType, primaryKeyAttributeId, pkAttribute.DataType);

        // check pk nullability
        if (pkAttribute.IsNullable)
            throw new PrimaryKeyAttributeNullableException(primaryKeyAttributeId);

        // check for self-join
        if (foreignKeyTableauId.Equals(primaryKeyTableauId))
            throw new SelfJoinNotSupportedException(primaryKeyTableauId);

        Join join = new Join(primaryKeyTableauId, primaryKeyAttributeId, foreignKeyTableauId, foreignKeyAttributeId);

        // check for cycle joins, pk tableaus multiple references, duplicate joins
        if (_joining.Joins.Contains(join))
            throw new DuplicateJoinNotSupportedException(join);
        if (!JoiningUtils.ArePrimaryKeyReferencesUnique(_joining, join))
            throw new TableauPrimaryKeyReferenceNotUniqueException(join);
        if (JoiningUtils.IsJoiningCyclic(_initialTableauId, _joining, join))
            throw new CyclicJoinNotSupportedException(join);
        _joining.AddJoin(join);

        return this;
    }

    /// <summary>
    /// Builds the joining clause
    /// </summary>
    /// <returns>Joining</returns>
    /// <exception cref="JoinsNotConnectedException"></exception>
    public Joining Build()
    {
        if (!JoiningUtils.IsJoiningConnectedGraph(_initialTableauId, _joining)) 
            throw new JoinsNotConnectedException();
        return _joining;
    }
}





