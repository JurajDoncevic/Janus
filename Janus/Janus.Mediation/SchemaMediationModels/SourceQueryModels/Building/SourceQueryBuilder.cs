using Janus.Commons.SchemaModels;
using Janus.Mediation.SchemaMediationModels.Exceptions;
using Janus.Mediation.SchemaMediationModels.MediationQueryModels;
using Janus.Commons;

namespace Janus.Mediation.SchemaMediationModels.SourceQueryModels.Building;
/// <summary>
/// Mediation source query builder starting interface
/// </summary>
public interface ISourceQueryBuilder
{
    /// <summary>
    /// Sets the source query joining
    /// </summary>
    /// <param name="configuration">Joining configuration via the <see cref="JoiningBuilder"/></param>
    /// <returns>Current source query builder</returns>
    IPostJoiningQueryBuilder WithJoining(Func<JoiningBuilder, JoiningBuilder> configuration);
    /// <summary>
    /// Sets the source query projection
    /// </summary>
    /// <param name="configuration">Projection configuration via the <see cref="ProjectionBuilder"/></param>
    /// <returns>Current source query builder</returns>
    IPostProjectionQueryBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration);
}
/// <summary>
/// Mediation source query builder interface after the joining has been set
/// </summary>
public interface IPostJoiningQueryBuilder
{
    /// <summary>
    /// Sets the source query projection
    /// </summary>
    /// <param name="configuration">Projection configuration via the <see cref="ProjectionBuilder"/></param>
    /// <returns>Current source query builder</returns>
    IPostProjectionQueryBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration);
}
/// <summary>
/// Mediation source query builder interface after the projection has been set
/// </summary>
public interface IPostProjectionQueryBuilder
{
    /// <summary>
    /// Sets the source query projection
    /// </summary>
    /// <param name="configuration">Projection configuration via the <see cref="ProjectionBuilder"/></param>
    /// <returns>Current source query builder</returns>
    IPostProjectionQueryBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration);
    /// <summary>
    /// Builds the currently setup source query
    /// </summary>
    /// <returns>Source query object</returns>
    SourceQuery Build();
}

/// <summary>
/// Default source query builder implementation
/// </summary>
public class SourceQueryBuilder : ISourceQueryBuilder, IPostJoiningQueryBuilder, IPostProjectionQueryBuilder
{
    private readonly Dictionary<string, DataSource> _availableDataSources;
    private Option<Joining> _joining;
    private string _initialTableauId;
    private HashSet<string> _referencedTableauIds;
    private Projection? _projection;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="initialTableauId">Initial source query tableau id</param>
    /// <param name="availableDataSources">Available data sources for the query</param>
    /// <exception cref="ArgumentException"></exception>
    private SourceQueryBuilder(string initialTableauId, IEnumerable<DataSource> availableDataSources)
    {
        if (string.IsNullOrWhiteSpace(initialTableauId))
        {
            throw new ArgumentException($"'{nameof(initialTableauId)}' cannot be null or whitespace.", nameof(initialTableauId));
        }

        _initialTableauId = initialTableauId;
        _availableDataSources = availableDataSources?.ToDictionary(ds => ds.Name, ds => ds) ?? new Dictionary<string, DataSource>();

        _referencedTableauIds = new HashSet<string>();
        _referencedTableauIds.Add(initialTableauId);
    }
    /// <summary>
    /// Initializes the source query on a tableau with the given id, having the available data sources to query
    /// </summary>
    /// <param name="initialTableauId"></param>
    /// <param name="availableDataSources"></param>
    /// <returns></returns>
    public static ISourceQueryBuilder InitQueryOn(string initialTableauId, IEnumerable<DataSource> availableDataSources)
    {
        return new SourceQueryBuilder(initialTableauId, availableDataSources);
    }

    public IPostJoiningQueryBuilder WithJoining(Func<JoiningBuilder, JoiningBuilder> configuration)
    {
        var builder = new JoiningBuilder(_initialTableauId, _availableDataSources.Values);
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

    public IPostProjectionQueryBuilder WithProjection(Func<ProjectionBuilder, ProjectionBuilder> configuration)
    {
        var builder = new ProjectionBuilder(_availableDataSources.Values, _referencedTableauIds);
        builder = configuration(builder);
        _projection = builder.IsConfigured
                     ? builder.Build()
                     : null;
        return this;
    }

    public SourceQuery Build()
    {
        if (_projection == null)
        {
            throw new InvalidOperationException("Projection can't be null");
        }

        return new SourceQuery(_initialTableauId, _joining, _projection!);
    }
}

/// <summary>
/// Builder class for mediation query projection
/// </summary>
public class ProjectionBuilder
{
    private readonly Dictionary<string, DataSource> _dataSources;
    private HashSet<string> _projectionAttributes;
    private readonly HashSet<string> _referencedTableauIds;
    internal bool IsConfigured => _projectionAttributes.Count > 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableauIds">Ids of tableaus referenced in the query</param>
    internal ProjectionBuilder(IEnumerable<DataSource> dataSources, HashSet<string> referencedTableauIds)
    {
        _dataSources = dataSources?.ToDictionary(ds => ds.Name, ds => ds) ?? throw new ArgumentNullException(nameof(dataSources));
        _projectionAttributes = new HashSet<string>();
        _referencedTableauIds = referencedTableauIds ?? throw new ArgumentNullException(nameof(referencedTableauIds));
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
        if (!_dataSources.Any(ds => ds.Value.ContainsAttribute(attributeId)))
            throw new AttributeDoesNotExistException(attributeId, _dataSources.Select(ds => ds.Value.Name));
        if (!_dataSources.SelectMany(ds => ds.Value.Schemas)
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
/// Builder class for mediation query joins
/// </summary>
public class JoiningBuilder
{
    private readonly string _initialTableauId;
    private readonly Dictionary<string, DataSource> _dataSources;
    private Joining _joining;

    internal bool IsConfigured => _joining.Joins.Count > 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    internal JoiningBuilder(string initialTableauId, IEnumerable<DataSource> dataSources)
    {
        if (string.IsNullOrEmpty(initialTableauId))
        {
            throw new ArgumentException($"'{nameof(initialTableauId)}' cannot be null or empty.", nameof(initialTableauId));
        }

        _initialTableauId = initialTableauId;
        _dataSources = dataSources?.ToDictionary(ds => ds.Name, ds => ds) ?? throw new ArgumentNullException(nameof(dataSources));
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
        if (!_dataSources.Any(ds => ds.Value.ContainsAttribute(foreignKeyAttributeId)))
            throw new AttributeDoesNotExistException(foreignKeyAttributeId, _dataSources.Values.Select(ds => ds.Name));
        if (!_dataSources.Any(ds => ds.Value.ContainsAttribute(primaryKeyAttributeId)))
            throw new AttributeDoesNotExistException(primaryKeyAttributeId, _dataSources.Values.Select(ds => ds.Name));

        // check attribute types - must be the same
        var fkNames = Utils.GetNamesFromAttributeId(foreignKeyAttributeId);
        var pkNames = Utils.GetNamesFromAttributeId(primaryKeyAttributeId);
        var fkAttribute = _dataSources[fkNames.dataSourceName][fkNames.schemaName][fkNames.tableauName][fkNames.attributeName];
        var pkAttribute = _dataSources[pkNames.dataSourceName][pkNames.schemaName][pkNames.tableauName][pkNames.attributeName];
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
