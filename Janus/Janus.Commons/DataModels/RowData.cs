namespace Janus.Commons.DataModels;

/// <summary>
/// Describes a row of tableau data
/// </summary>
public sealed class RowData
{
    private readonly Dictionary<string, object?> _columnValues;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attributeValues">Values of attributes in the row</param>
    internal RowData(Dictionary<string, object?> columnValues)
    {
        _columnValues = columnValues ?? throw new ArgumentNullException(nameof(columnValues));
    }

    /// <summary>
    /// Values of attributes in the row
    /// </summary>
    public IReadOnlyDictionary<string, object?> ColumnValues => _columnValues;

    public object this[string columnName] => _columnValues[columnName];

    public override string ToString()
        => "(" + string.Join(";", _columnValues.Values) + ")";

    public override bool Equals(object? obj)
    {
        return obj is RowData data &&
               _columnValues.SequenceEqual(data._columnValues);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_columnValues);
    }
}
