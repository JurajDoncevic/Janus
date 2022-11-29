using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.QueryModels.Building;

/// <summary>
/// Builder class for query projection
/// </summary>
public sealed class ProjectionBuilder
{
    private readonly DataSource _dataSource;
    private HashSet<AttributeId> _projectionAttributes;
    private readonly HashSet<TableauId> _referencedTableauIds;
    internal bool IsConfigured => _projectionAttributes.Count > 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dataSource">Data source on which the query will be executed</param>
    /// <param name="referencedTableauIds">Ids of tableaus referenced in the query</param>
    internal ProjectionBuilder(DataSource dataSource, HashSet<TableauId> referencedTableauIds)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _projectionAttributes = new HashSet<AttributeId>();
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
    public ProjectionBuilder AddAttribute(AttributeId attributeId)
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
    /// Adds an attribute to the projection
    /// </summary>
    /// <param name="attributeId">Attribute id</param>
    /// <returns>ProjectionBuilder</returns>
    public ProjectionBuilder AddAttribute(string attributeId)
        => AddAttribute(AttributeId.From(attributeId));

    /// <summary>
    /// Builds the specified projection
    /// </summary>
    /// <returns>Projection</returns>
    internal Projection Build()
    {
        return new Projection(_projectionAttributes);
    }
}





