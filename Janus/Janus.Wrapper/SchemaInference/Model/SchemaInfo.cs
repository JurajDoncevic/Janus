namespace Janus.Wrapper.SchemaInference.Model;
public class SchemaInfo
{
    private readonly string _name;

    public SchemaInfo(string name)
    {
        _name = name;
    }

    public string Name => _name;
}
