using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels;

#region OPEN QUERY BUILDER SEQUENCE INTERFACES

/// <summary>
/// Determines what declarations can be made after an open query initialization
/// </summary>
internal interface IPostInitOpenBuilder
{
    /// <summary>
    /// Specifies a joining clause of the query
    /// </summary>
    /// <param name="configuration">Joining configuration over a <see cref="JoiningBuilder"/></param>
    /// <returns>IPostInitOpenBuilder</returns>
    public IPostInitOpenBuilder WithJoining(Func<JoiningOpenBuilder, JoiningOpenBuilder> configuration);

    /// <summary>
    /// Specifies a projection clause of the query 
    /// </summary>
    /// <param name="configuration">Projection configuration over a <see cref="ProjectionBuilder"/></param>
    /// <returns>IPostInitOpenBuilder</returns>
    public IPostInitOpenBuilder WithProjection(Func<ProjectionOpenBuilder, ProjectionOpenBuilder> configuration);

    /// <summary>
    /// Specifies a selection clause of the query
    /// </summary>
    /// <param name="configuration">Selection configuration over a <see cref="SelectionBuilder"/></param>
    /// <returns>IPostInitOpenBuilder</returns>
    public IPostInitOpenBuilder WithSelection(Func<SelectionOpenBuilder, SelectionOpenBuilder> configuration);

    /// <summary>
    /// Builds the specified query
    /// </summary>
    /// <returns>Query</returns>
    public Query Build();
}
#endregion

internal class QueryModelOpenBuilder : IPostInitOpenBuilder
{
    private Option<Projection> _projection;
    private Option<Selection> _selection;
    private Option<Joining> _joining;
    private readonly string _queryOnTableauId;
    private HashSet<string> _referencedTableaus;

    /// <summary>
    /// Constructor. Used when build-time validation is NOT required.
    /// </summary>
    private QueryModelOpenBuilder(string queryOnTableauId!!)
    {
        _projection = Option<Projection>.None;
        _selection = Option<Selection>.None;
        _joining = Option<Joining>.None;
        _queryOnTableauId = queryOnTableauId;
        _referencedTableaus = new HashSet<string>();
        _referencedTableaus.Add(queryOnTableauId);
    }

    /// <summary>
    /// Initializes the query on a tableau found in the given data source. Generated query is valid on the data source.
    /// </summary>
    /// <param name="onTableauId">Id of tableau on which the query is initialized</param>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <returns>QueryOpenModelBuilder</returns>
    internal static QueryModelOpenBuilder InitOpenQuery(string onTableauId!!)
    {
        return new QueryModelOpenBuilder(onTableauId);
    }

    /// <summary>
    /// Specifies a joining clause of the query
    /// </summary>
    /// <param name="configuration">Joining configuration over a <see cref="JoiningBuilder"/></param>
    /// <returns>IPostInitOpenBuilder</returns>
    public IPostInitOpenBuilder WithJoining(Func<JoiningOpenBuilder, JoiningOpenBuilder> configuration)
    {
        var builder = new JoiningOpenBuilder();
        builder = configuration(builder);
        _joining = builder.IsConfigured
                   ? Option<Joining>.Some(builder.Build())
                   : Option<Joining>.None;

        return this;
    }

    /// <summary>
    /// Specifies a projection clause of the query 
    /// </summary>
    /// <param name="configuration">Projection configuration over a <see cref="ProjectionBuilder"/></param>
    /// <returns>IPostInitOpenBuilder</returns>
    public IPostInitOpenBuilder WithProjection(Func<ProjectionOpenBuilder, ProjectionOpenBuilder> configuration)
    {
        var builder = new ProjectionOpenBuilder();
        builder = configuration(builder);
        _projection = builder.IsConfigured
                      ? Option<Projection>.Some(builder.Build())
                      : Option<Projection>.None;

        return this;
    }

    /// <summary>
    /// Specifies a selection clause of the query
    /// </summary>
    /// <param name="configuration">Selection configuration over a <see cref="SelectionBuilder"/></param>
    /// <returns>IPostInitOpenBuilder</returns>
    public IPostInitOpenBuilder WithSelection(Func<SelectionOpenBuilder, SelectionOpenBuilder> configuration)
    {
        var builder = new SelectionOpenBuilder();
        builder = configuration(builder);
        _selection = builder.IsConfigured
                     ? Option<Selection>.Some(builder.Build())
                     : Option<Selection>.None;

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
internal class SelectionOpenBuilder
{
    private SelectionExpression _expression;
    internal bool IsConfigured => _expression != null;

    /// <summary>
    /// Constructor
    /// </summary>
    internal SelectionOpenBuilder()
    {
        _expression = null;
    }

    /// <summary>
    /// Creates the selection expression
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>SelectionBuilder</returns>
    internal SelectionOpenBuilder WithExpression(SelectionExpression expression!!)
    {
        _expression = expression;
        return this;
    }

    /// <summary>
    /// Builds the specified selection
    /// </summary>
    /// <returns></returns>
    internal Selection Build()
    {
        return new Selection(_expression ?? new TrueLiteral());
    }
}

/// <summary>
/// Builder class for query projection
/// </summary>
internal class ProjectionOpenBuilder
{
    private HashSet<string> _projectionAttributes;

    internal bool IsConfigured => _projectionAttributes.Count > 0;

    /// <summary>
    /// Constructor
    /// </summary>
    internal ProjectionOpenBuilder()
    {
        _projectionAttributes = new HashSet<string>();
    }

    /// <summary>
    /// Adds an attribute to the projection
    /// </summary>
    /// <param name="attributeId">Attribute id</param>
    /// <returns>ProjectionBuilder</returns>
    internal ProjectionOpenBuilder AddAttribute(string attributeId)
    {
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
internal class JoiningOpenBuilder
{
    private Joining _joining;

    internal bool IsConfigured => _joining.Joins.Count > 0;

    /// <summary>
    /// Constructor
    /// </summary>
    internal JoiningOpenBuilder()
    {
        _joining = new Joining();
    }

    /// <summary>
    /// Adds a join to the joining clause
    /// </summary>
    /// <returns></returns>
    internal JoiningOpenBuilder AddJoin(string foreignKeyAttributeId, string primaryKeyAttributeId)
    {
        // check if given ids are ok
        if (!foreignKeyAttributeId.Contains('.'))
            throw new InvalidAttributeIdException(foreignKeyAttributeId);
        if (!primaryKeyAttributeId.Contains('.'))
            throw new InvalidAttributeIdException(primaryKeyAttributeId);

        // get tableau ids from the supposed attribute ids
        var foreignKeyTableauId = foreignKeyAttributeId.Remove(foreignKeyAttributeId.LastIndexOf('.'));
        var primaryKeyTableauId = primaryKeyAttributeId.Remove(primaryKeyAttributeId.LastIndexOf('.'));

        Join join = new Join(primaryKeyTableauId, primaryKeyAttributeId, foreignKeyTableauId, foreignKeyAttributeId);

        _joining.AddJoin(join);

        return this;
    }

    /// <summary>
    /// Builds the joining clause
    /// </summary>
    /// <returns>Joining</returns>
    internal Joining Build()
    {
        return _joining;
    }
}

