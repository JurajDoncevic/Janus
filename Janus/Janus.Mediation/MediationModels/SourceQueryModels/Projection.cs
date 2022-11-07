namespace Janus.Mediation.MediationModels.MediationQueryModels;

/// <summary>
/// Describes a projection clause
/// </summary>
public class Projection
{
    private HashSet<string> _includedAttributeIds;
    public IReadOnlyCollection<string> IncludedAttributeIds => _includedAttributeIds;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="includedAttributeIds">Attribute ids to be included in the projection</param>
    internal Projection(HashSet<string> includedAttributeIds)
    {
        _includedAttributeIds = includedAttributeIds ?? throw new ArgumentNullException(nameof(includedAttributeIds));
    }

    /// <summary>
    /// Constructor
    /// </summary>
    internal Projection()
    {
        _includedAttributeIds = new HashSet<string>();
    }

    /// <summary>
    /// Adds attribute to projection
    /// </summary>
    /// <param name="attributeId">Attribute id</param>
    /// <returns></returns>
    internal bool AddAttribute(string attributeId)
    {
        return _includedAttributeIds.Add(attributeId);
    }

    /// <summary>
    /// Removes attribute from projection
    /// </summary>
    /// <param name="attributeId">Attribute id</param>
    /// <returns></returns>
    internal bool RemoveAttribute(string attributeId)
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
