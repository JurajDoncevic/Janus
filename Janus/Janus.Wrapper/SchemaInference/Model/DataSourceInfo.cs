namespace Janus.Wrapper.SchemaInference.Model;
public class DataSourceInfo
{
    private readonly string _name;

    public DataSourceInfo(string name)
    {
        _name = name;
    }

    public string Name => _name;
}
