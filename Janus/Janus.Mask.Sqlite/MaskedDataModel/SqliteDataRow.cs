namespace Janus.Mask.Sqlite.MaskedDataModel;

public sealed class SqliteDataRow
{
    private readonly Dictionary<string, object?> _dataRow;
    
    public SqliteDataRow(Dictionary<string, object?> dataRow)
    {
        _dataRow = dataRow;
    }

    public IReadOnlyDictionary<string, object?> DataRow => _dataRow;
}