using Janus.Commons.SchemaModels.Exceptions;

namespace Janus.Commons.SchemaModels.Building;
public class UpdateSetBuilder : IUpdateSetBuilder
{
    private HashSet<string> _attributeNames;
    private readonly Tableau _parentTableau;

    private IReadOnlySet<string> AttributeIds => _attributeNames.Map(attrName => $"{_parentTableau.Id}.{attrName}").ToHashSet();

    public UpdateSetBuilder(Tableau parentTableau)
    {
        _parentTableau = parentTableau ?? throw new ArgumentNullException(nameof(parentTableau));
        _attributeNames = new HashSet<string>();
    }

    public UpdateSet Build()
    {
        return new UpdateSet(_attributeNames ?? new HashSet<string>(), _parentTableau);
    }

    public IUpdateSetBuilder WithAttributesNamed(params string[] attributeNames)
    {
        if (attributeNames is null || attributeNames.Count() == 0)
        {
            throw new UpdateSetEmptyException();
        }
        var attrNamesInTableau = _parentTableau.AttributeNames;
        if (!attributeNames.All(attrNamesInTableau.Contains))
        {
            var unknownAttrName = _attributeNames.FirstOrDefault(attrId => !attrNamesInTableau.Contains(attrId));
            throw new UpdateSetAttributeDoesNotExist(unknownAttrName ?? string.Empty, _parentTableau.Name);
        }
        _attributeNames = new HashSet<string>(attributeNames);
        return this;
    }
    public IUpdateSetBuilder WithAttributesNamed(IEnumerable<string> attributeNames)
    {
        if (attributeNames is null || attributeNames.Count() == 0)
        {
            throw new UpdateSetEmptyException();
        }
        var attrIdsInTableau = _parentTableau.Attributes.Map(attr => attr.Id);
        if (!AttributeIds.All(attrIdsInTableau.Contains))
        {
            var unknownAttrId = _attributeNames.FirstOrDefault(attrId => !attrIdsInTableau.Contains(attrId));
            throw new UpdateSetAttributeDoesNotExist(unknownAttrId ?? string.Empty, _parentTableau.Name);
        }
        _attributeNames = new HashSet<string>(attributeNames);
        return this;
    }

}


public interface IUpdateSetBuilder : IUpdateSetBuilding
{
    public IUpdateSetBuilder WithAttributesNamed(params string[] attributeNames);
    public IUpdateSetBuilder WithAttributesNamed(IEnumerable<string> attributeNames);
}

public interface IUpdateSetBuilding
{
    /// <summary>
    /// Builds the update set
    /// </summary>
    /// <returns>Built UpdateSet</returns>
    public UpdateSet Build();
}