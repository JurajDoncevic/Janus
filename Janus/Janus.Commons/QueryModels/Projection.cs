
namespace Janus.Commons.QueryModels;

public class Projection
{
    private HashSet<string> _includedAttributeIds;
    public IReadOnlyCollection<string> IncludedAttributeIds => _includedAttributeIds;

    internal Projection(HashSet<string> includedAttributeIds!!)
    {
        _includedAttributeIds = includedAttributeIds;
    }

    internal Projection()
    {
        _includedAttributeIds = new HashSet<string>();
    }

    internal bool AddAttribute(string attributeId)
    {
        return _includedAttributeIds.Add(attributeId);
    }

    internal bool RemoveAttribute(string attributeId)
    {
        return _includedAttributeIds.Remove(attributeId);
    }

}
