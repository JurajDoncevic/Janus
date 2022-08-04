namespace Janus.Wrapper.Sqlite.LocalDataModel;
internal class SqliteTabularData
{
    public Dictionary<string, Type> DataSchema => Data.FirstOrDefault()?.ToDictionary(kv => kv.Key, kv => kv.Value.Item1) ?? new Dictionary<string, Type>();
    public List<Dictionary<string, (Type, object)>> Data { get; init; } = new List<Dictionary<string, (Type, object)>>();
}
