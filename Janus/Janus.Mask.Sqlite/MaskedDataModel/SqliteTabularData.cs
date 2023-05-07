using Janus.Mask.MaskedDataModel;
using Janus.Mask.Sqlite.MaskedSchemaModel;

namespace Janus.Mask.Sqlite.MaskedDataModel;
public sealed class SqliteTabularData : MaskedData<SqliteDataRow>
{
    private readonly List<SqliteDataRow> _dataRows;
    private readonly Dictionary<string, TypeAffinities> _dataSchema;
    public SqliteTabularData(List<SqliteDataRow> dataRows, Dictionary<string, TypeAffinities> dataSchema)
    {
        _dataRows = dataRows ?? new List<SqliteDataRow>();
        _dataSchema = dataSchema ?? new Dictionary<string, TypeAffinities>();
    }
    public IReadOnlyDictionary<string, TypeAffinities> DataSchema => _dataSchema;

    public override IReadOnlyList<SqliteDataRow> Data => _dataRows;

    public override long ItemCount => _dataRows.Count;
}
