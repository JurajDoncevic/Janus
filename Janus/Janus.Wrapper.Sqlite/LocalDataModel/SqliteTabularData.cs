using System.Collections.ObjectModel;

namespace Janus.Wrapper.Sqlite.LocalDataModel;
public sealed class SqliteTabularData
{

    private readonly Dictionary<string, Type> _dataSchema; 
    private readonly List<Dictionary<string, object?>> _dataRows;

    public SqliteTabularData(Dictionary<string, Type> dataSchema, List<Dictionary<string, object?>> data)
    {
        _dataSchema = dataSchema ?? new Dictionary<string, Type>();
        _dataRows = data ?? new List<Dictionary<string, object?>>();
    }

    public bool SchemaHasColumns => _dataSchema.Count > 0;

    public IReadOnlyDictionary<string, Type> DataSchema => new ReadOnlyDictionary<string, Type>(_dataSchema);

    public IReadOnlyList<Dictionary<string, object?>> DataRows => _dataRows;
}
