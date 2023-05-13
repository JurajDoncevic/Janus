namespace Janus.Mask.WebApi.MaskedSchemaModel;
public class DtoTyping
{
    private readonly string _prefix = string.Empty;
    private readonly string _name;
    private readonly Type _idPropertyType;
    private readonly Dictionary<string, Type> _properties;

    public DtoTyping(string name,
                     Type idPropertyType,
                     Dictionary<string, Type> properties,
                     string? prefix = null)
    {
        _name = name;
        _idPropertyType = idPropertyType;
        _properties = properties;
        _prefix = prefix ?? _prefix;
    }

    public string Name => _name;

    public Type IdPropertyType => _idPropertyType;

    public IReadOnlyDictionary<string, Type> Properties => _properties;

    public string Prefix => _prefix;
}
