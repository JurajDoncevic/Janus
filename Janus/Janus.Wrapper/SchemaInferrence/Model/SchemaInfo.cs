namespace Janus.Wrapper.SchemaInferrence.Model;
public class SchemaInfo
{
    private readonly string _name;

    public SchemaInfo(string name)
    {
        _name = name;
    }

    public string Name => _name;
}
