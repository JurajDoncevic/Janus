using Janus.Commons.SchemaModels;

namespace Janus.Commons.DataModels;

/// <summary>
/// Describes tabular data
/// </summary>
public sealed class TabularData
{
    private readonly string _name;
    private readonly List<RowData> _rowData;
    private readonly Dictionary<string, DataTypes> _columnDataTypes;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="columnDataTypes">Data types for columns</param>
    internal TabularData(Dictionary<string, DataTypes> columnDataTypes, string? name = null)
    {
        _name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
        _columnDataTypes = columnDataTypes;
        _rowData = new List<RowData>();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rowData">Rows of data to be placed into tabular data</param>
    /// <param name="columnDataTypes">Data types for attributes</param>
    internal TabularData(IEnumerable<RowData> rowData, Dictionary<string, DataTypes> columnDataTypes, string? name = null)
    {
        _name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
        _rowData = rowData.ToList();
        _columnDataTypes = columnDataTypes;
    }

    /// <summary>
    /// Adds a row to the tabular data
    /// </summary>
    /// <param name="rowData"></param>
    internal void AddRow(RowData rowData)
    {
        _rowData.Add(rowData);
    }

    /// <summary>
    /// Rows of tabular data
    /// </summary>
    public IReadOnlyList<RowData> RowData => _rowData;

    /// <summary>
    /// Data types of attributes in the tabular data
    /// </summary>
    public IReadOnlyDictionary<string, DataTypes> ColumnDataTypes => _columnDataTypes;

    /// <summary>
    /// Row data by number index
    /// </summary>
    /// <param name="index"></param>
    /// <returns>Row data instance</returns>
    public RowData this[int index] => _rowData[index];

    /// <summary>
    /// Attribute names in the tabular
    /// </summary>
    public HashSet<string> ColumnNames => _columnDataTypes.Keys.ToHashSet();

    /// <summary>
    /// Identifier for this instance of tabular data
    /// </summary>
    public string Name => _name;

    public override string ToString()
        => string.Join("\n", _rowData);

    public override bool Equals(object? obj)
    {
        return obj is TabularData data &&
            _name == data._name &&
            _columnDataTypes.SequenceEqual(data._columnDataTypes) &&
            _rowData.SequenceEqual(data._rowData);

    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_rowData, _columnDataTypes, _name);
    }
}
