using Janus.Commons.QueryModels.Building;
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
    /// Sets the query name as a identifier
    /// </summary>
    /// <param name="queryName">Query name</param>
    /// <returns>Current builder</returns>
    public IPostInitBuilder WithName(string queryName);

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
public sealed class QueryModelBuilder : IPostInitBuilder, IPostJoiningBuilder, IPostSelectionBuilder, IPostProjectionBuilder
{
    private Option<Projection> _projection;
    private Option<Selection> _selection;
    private Option<Joining> _joining;
    private readonly TableauId _queryOnTableauId;
    private readonly DataSource _dataSource;
    private HashSet<TableauId> _referencedTableauIds;
    private string? _queryName;

    /// <summary>
    /// Constructor. Used when build-time validation is required.
    /// </summary>
    /// <param name="queryOnTableauId">Id of tableau on which the query is initialized</param>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    private QueryModelBuilder(TableauId queryOnTableauId, DataSource dataSource)
    {
        if (queryOnTableauId is null)
        {
            throw new ArgumentException($"'{nameof(queryOnTableauId)}' cannot be null or empty.", nameof(queryOnTableauId));
        }

        _projection = Option<Projection>.None;
        _selection = Option<Selection>.None;
        _joining = Option<Joining>.None;
        _queryOnTableauId = queryOnTableauId;
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _referencedTableauIds = new HashSet<TableauId>();
        _referencedTableauIds.Add(queryOnTableauId);
    }


    /// <summary>
    /// Initializes the query on a tableau found in the given data source. Generated query is valid on the data source.
    /// </summary>
    /// <param name="onTableauId">Id of tableau on which the query is initialized</param>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <returns>QueryModelBuilder</returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitBuilder InitQueryOnDataSource(TableauId onTableauId, DataSource dataSource)
    {
        if (onTableauId is null)
        {
            throw new ArgumentException($"'{nameof(onTableauId)}' cannot be null or empty.", nameof(onTableauId));
        }

        if (dataSource is null)
        {
            throw new ArgumentNullException(nameof(dataSource));
        }

        if (!dataSource.ContainsTableau(onTableauId))
            throw new TableauDoesNotExistException(onTableauId, dataSource.Name);

        return new QueryModelBuilder(onTableauId, dataSource);
    }

    /// <summary>
    /// Initializes the query on a tableau found in the given data source. Generated query is valid on the data source.
    /// </summary>
    /// <param name="onTableauId">Id of tableau on which the query is initialized</param>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <returns>QueryModelBuilder</returns>
    /// <exception cref="TableauDoesNotExistException"></exception>
    public static IPostInitBuilder InitQueryOnDataSource(string onTableauId, DataSource dataSource)
        => InitQueryOnDataSource(TableauId.From(onTableauId), dataSource);

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
        return new Query(_queryOnTableauId, _projection, _selection, _joining, _queryName);
    }

    public IPostInitBuilder WithName(string queryName)
    {
        _queryName = queryName;

        return this;
    }
}





