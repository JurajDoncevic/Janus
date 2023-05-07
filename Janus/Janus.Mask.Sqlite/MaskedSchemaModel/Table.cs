namespace Janus.Mask.Sqlite.MaskedSchemaModel;
public sealed class Table
{
    private readonly string _name;
    private readonly Dictionary<string, Column> _columns;

    internal Table(string name, IEnumerable<Column>? columns = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }

        _name = name;
        _columns = columns?.ToDictionary(_ => _.Name, _ => _) ?? new Dictionary<string, Column>();
    }

    public string Name => _name;

    public IReadOnlyList<Column> Columns => _columns.Values.ToList();

    public Column this[string columnName] => _columns[columnName];

    internal bool AddColumn(Column column)
    {
        if (column is null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        if (_columns.Values.ToList().Exists(c => c.Ordinal == column.Ordinal))
        {
            return false;
        }
        return _columns.TryAdd(column.Name, column);
    }

    internal bool RemoveColumn(string columnName)
    {
        if (columnName is null)
        {
            throw new ArgumentNullException(nameof(columnName));
        }

        return _columns.Remove(columnName);
    }
}
