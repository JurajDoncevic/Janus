using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels;

/// <summary>
/// Signifies a group of attributes that can be update together
/// </summary>
public class UpdateSet
{
    private readonly HashSet<string> _attributeNames;
    private readonly Tableau _parentTableau;

    /// <summary>
    /// Names of attributes belonging to this update group
    /// </summary>
    public IReadOnlySet<string> AttributeNames => _attributeNames;

    /// <summary>
    /// Attribute IDs of attributes belonging to this update group
    /// </summary>
    public IReadOnlySet<string> AttributeIds => _attributeNames.Map(attrName => $"{_parentTableau.Id}.{attrName}").ToHashSet();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attributeNames">Attribute IDs in the update group</param>
    internal UpdateSet(HashSet<string> attributeNames, Tableau parentTableau)
    {
        if(parentTableau is null)
            throw new ArgumentNullException(nameof(parentTableau));

        if (attributeNames is null || attributeNames.Count == 0)
            throw new UpdateSetEmptyException();

        _attributeNames = attributeNames;
        _parentTableau = parentTableau;
    }

    /// <summary>
    /// Determines if the update sets overlap
    /// </summary>
    /// <param name="updateSet">Other update set</param>
    /// <returns>True on overlap, else false</returns>
    public bool OverlapsWith(UpdateSet updateSet)
    {
        return updateSet.AttributeIds.Overlaps(AttributeIds);
    }

    /// <summary>
    /// Determines if the update set is empty
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return _attributeNames.Count == 0;
    }

    public override bool Equals(object? obj)
    {
        return obj is UpdateSet set &&
               _attributeNames.SetEquals(set._attributeNames);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_attributeNames);
    }

    public override string ToString()
    {
        return $"({string.Join(" ", AttributeNames)})";
    }
}
