using Janus.Commons.QueryModels.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels;

#region OPEN QUERY BUILDER SEQUENCE INTERFACES
public interface IPostInitOpenBuilder
{
    public IPostInitOpenBuilder WithJoining(Func<JoiningOpenBuilder, JoiningOpenBuilder> configuration);
    public IPostInitOpenBuilder WithProjection(Func<ProjectionOpenBuilder, ProjectionOpenBuilder> configuration);
    public IPostInitOpenBuilder WithSelection(Func<SelectionOpenBuilder, SelectionOpenBuilder> configuration);
    public Query Build();
}
#endregion
public class QueryModelOpenBuilder : IPostInitOpenBuilder
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
    public static QueryModelOpenBuilder InitOpenQuery(string onTableauId!!)
    {
        return new QueryModelOpenBuilder(onTableauId);
    }

    public IPostInitOpenBuilder WithJoining(Func<JoiningOpenBuilder, JoiningOpenBuilder> configuration)
    {
        var builder = new JoiningOpenBuilder();
        builder = configuration(builder);
        _joining = builder.IsConfigured
                   ? Option<Joining>.Some(builder.Build())
                   : Option<Joining>.None;

        return this;
    }

    public IPostInitOpenBuilder WithProjection(Func<ProjectionOpenBuilder, ProjectionOpenBuilder> configuration)
    {
        var builder = new ProjectionOpenBuilder();
        builder = configuration(builder);
        _projection = builder.IsConfigured
                      ? Option<Projection>.Some(builder.Build())
                      : Option<Projection>.None;

        return this;
    }

    public IPostInitOpenBuilder WithSelection(Func<SelectionOpenBuilder, SelectionOpenBuilder> configuration)
    {
        var builder = new SelectionOpenBuilder();
        builder = configuration(builder);
        _selection = builder.IsConfigured
                     ? Option<Selection>.Some(builder.Build())
                     : Option<Selection>.None;

        return this;
    }

    public Query Build()
    {
        return new Query(_queryOnTableauId, _projection, _selection, _joining);
    }
}

/// <summary>
/// Builder class for query selection
/// </summary>
public class SelectionOpenBuilder
{
    private string _expression;
    internal bool IsConfigured => !string.IsNullOrEmpty(_expression);

    /// <summary>
    /// Constructor
    /// </summary>
    internal SelectionOpenBuilder()
    {
        _expression = "";
    }

    /// <summary>
    /// Creates the selection expression
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>SelectionBuilder</returns>
    public SelectionOpenBuilder WithExpression(string expression!!)
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
public class ProjectionOpenBuilder
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
    public ProjectionOpenBuilder AddAttribute(string attributeId)
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
public class JoiningOpenBuilder
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
    public JoiningOpenBuilder AddJoin(string foreignKeyAttributeId, string primaryKeyAttributeId)
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
    public Joining Build()
    {
        return _joining;
    }
}

