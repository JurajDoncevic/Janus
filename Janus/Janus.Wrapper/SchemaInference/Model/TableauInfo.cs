namespace Janus.Wrapper.SchemaInference.Model;
public class TableauInfo
{
    private readonly string _name;

    public TableauInfo(string name)
    {
        _name = name;
    }

    public string Name => _name;
}
