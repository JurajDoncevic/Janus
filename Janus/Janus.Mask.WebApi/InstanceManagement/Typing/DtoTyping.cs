namespace Janus.Mask.WebApi.InstanceManagement.Typing;
public class DtoTyping
{
    private readonly string _name;
    private readonly Type _idPropertyType;
    private readonly Dictionary<string, Type> _properties;

    public DtoTyping(string name,
                     Type idPropertyType,
                     Dictionary<string, Type> properties)
    {
        _name = name;
        _idPropertyType = idPropertyType;
        _properties = properties;
    }

    public string Name => _name;

    public Type IdPropertyType => _idPropertyType;

    public IReadOnlyDictionary<string, Type> Properties => _properties;

}
