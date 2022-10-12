using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels;

/// <summary>
/// Signifies a group of attributes that can be update together
/// </summary>
public class UpdateSet
{
    private readonly HashSet<string> _attributeIds;

    /// <summary>
    /// Attribute IDs of attributes belonging to this update group
    /// </summary>
    public HashSet<string> AttributeIds => _attributeIds;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attributeIds">Attribute IDs in the update group</param>
    internal UpdateSet(HashSet<string> attributeIds)
    {
        if (_attributeIds is null || _attributeIds.Count == 0)
            throw new UpdateSetEmptyException();
        _attributeIds = attributeIds;
    }

    /// <summary>
    /// Determines if the update sets overlap
    /// </summary>
    /// <param name="updateSet">Other update set</param>
    /// <returns>True on overlap, else false</returns>
    public bool OverlapsWith(UpdateSet updateSet)
    {
        return updateSet.AttributeIds.Overlaps(_attributeIds);
    }

    /// <summary>
    /// Determines if the update set is empty
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return _attributeIds.Count == 0;
    }

    public override bool Equals(object? obj)
    {
        return obj is UpdateSet set &&
               _attributeIds.SetEquals(set._attributeIds);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_attributeIds);
    }
}
