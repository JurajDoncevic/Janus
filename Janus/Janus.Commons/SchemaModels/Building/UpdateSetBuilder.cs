using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels.Building;
public class UpdateSetBuilder : IUpdateSetBuilder
{
    private HashSet<string> _setOfAttributeIds;
    private Tableau _parentTableau;

    public UpdateSetBuilder(Tableau parentTableau)
    {
        _parentTableau = parentTableau ?? throw new ArgumentNullException(nameof(parentTableau));
        _setOfAttributeIds = new HashSet<string>();
    }

    public UpdateSet Build()
    {
        return new UpdateSet(_setOfAttributeIds ?? new HashSet<string>());
    }

    public IUpdateSetBuilder FromEnumerable(IEnumerable<string> attributeIds)
    {
        if(attributeIds is null || attributeIds.Count() == 0)
        {
            throw new UpdateSetEmptyException();
        }
        var attrIdsInTableau = _parentTableau.Attributes.Select(attr => attr.Id).ToList();
        if (!_setOfAttributeIds.All(attrIdsInTableau.Contains))
        {
            var unknownAttrId = _setOfAttributeIds.FirstOrDefault(attrId => !attrIdsInTableau.Contains(attrId));
            throw new UpdateSetAttributeDoesNotExist(unknownAttrId ?? string.Empty, _parentTableau.Name);
        }
        _setOfAttributeIds = new HashSet<string>(attributeIds);
        return this;
    }
}


public interface IUpdateSetBuilder : IUpdateSetBuilding
{
    public IUpdateSetBuilder FromEnumerable(IEnumerable<string> values);
}

public interface IUpdateSetBuilding
{
    /// <summary>
    /// Builds the update set
    /// </summary>
    /// <returns>Built UpdateSet</returns>
    public UpdateSet Build();
}