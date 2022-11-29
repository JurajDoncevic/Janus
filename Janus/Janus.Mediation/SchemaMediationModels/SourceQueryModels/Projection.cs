using Janus.Commons.SchemaModels;

namespace Janus.Mediation.SchemaMediationModels.MediationQueryModels;

/// <summary>
/// Describes a projection clause
/// </summary>
public class Projection
{
    private HashSet<AttributeId> _includedAttributeIds;
    public IReadOnlyCollection<AttributeId> IncludedAttributeIds => _includedAttributeIds;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="includedAttributeIds">Attribute ids to be included in the projection</param>
    internal Projection(HashSet<AttributeId> includedAttributeIds)
    {
        _includedAttributeIds = includedAttributeIds ?? throw new ArgumentNullException(nameof(includedAttributeIds));
    }

    /// <summary>
    /// Constructor
    /// </summary>
    internal Projection()
    {
        _includedAttributeIds = new HashSet<AttributeId>();
    }

    /// <summary>
    /// Adds attribute to projection
    /// </summary>
    /// <param name="attributeId">Attribute id</param>
    /// <returns></returns>
    internal bool AddAttribute(AttributeId attributeId)
    {
        return _includedAttributeIds.Add(attributeId);
    }

    /// <summary>
    /// Removes attribute from projection
    /// </summary>
    /// <param name="attributeId">Attribute id</param>
    /// <returns></returns>
    internal bool RemoveAttribute(AttributeId attributeId)
    {
        return _includedAttributeIds.Remove(attributeId);
    }

    public override bool Equals(object? obj)
    {
        return obj is Projection projection &&
               projection.IncludedAttributeIds.SequenceEqual(_includedAttributeIds);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_includedAttributeIds);
    }
}
