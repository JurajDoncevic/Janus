using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Building;

/// <summary>
/// Builder class for query joins
/// </summary>
public sealed class JoiningBuilder
{
    private readonly TableauId _initialTableauId;
    private readonly DataSource _dataSource;
    private Joining _joining;

    internal bool IsConfigured => _joining.Joins.Count > 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    internal JoiningBuilder(TableauId initialTableauId, DataSource dataSource)
    {
        if (initialTableauId is null)
        {
            throw new ArgumentException($"'{nameof(initialTableauId)}' cannot be null or empty.", nameof(initialTableauId));
        }

        _initialTableauId = initialTableauId;
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
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
    public JoiningBuilder AddJoin(AttributeId foreignKeyAttributeId, AttributeId primaryKeyAttributeId)
    {
        // get tableau ids from the supposed attribute ids
        var foreignKeyTableauId = foreignKeyAttributeId.ParentTableauId;
        var primaryKeyTableauId = primaryKeyAttributeId.ParentTableauId;

        // check attribute (and tableau) existence
        if (!_dataSource.ContainsAttribute(foreignKeyAttributeId))
            throw new AttributeDoesNotExistException(foreignKeyAttributeId, _dataSource.Name);
        if (!_dataSource.ContainsAttribute(primaryKeyAttributeId))
            throw new AttributeDoesNotExistException(primaryKeyAttributeId, _dataSource.Name);

        // check attribute types - must be the same
        var fkNames = foreignKeyAttributeId.NameTuple;
        var pkNames = primaryKeyAttributeId.NameTuple;
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

        Join join = new Join(foreignKeyAttributeId, primaryKeyAttributeId);

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
    /// Adds a join to the joining clause
    /// </summary>
    /// <param name="foreignKeyAttributeId">Attribute id of the foreign key</param>
    /// <param name="primaryKeyAttributeId">Attribute id of the primary key</param>
    public JoiningBuilder AddJoin(string foreignKeyAttributeId, string primaryKeyAttributeId)
        => AddJoin(AttributeId.From(foreignKeyAttributeId), AttributeId.From(primaryKeyAttributeId));

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





