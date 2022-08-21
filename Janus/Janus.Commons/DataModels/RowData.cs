namespace Janus.Commons.DataModels;

/// <summary>
/// Describes a row of tableau data
/// </summary>
public class RowData
{
    private readonly Dictionary<string, object?> _attributeValues;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attributeValues">Values of attributes in the row</param>
    internal RowData(Dictionary<string, object?> attributeValues)
    {
        _attributeValues = attributeValues ?? throw new ArgumentNullException(nameof(attributeValues));
    }

    /// <summary>
    /// Values of attributes in the row
    /// </summary>
    public IReadOnlyDictionary<string, object?> AttributeValues => _attributeValues;

    public object this[string attributeId] => _attributeValues[attributeId];

    public override string ToString()
        => "(" + string.Join(";", _attributeValues.Values) + ")";

    public override bool Equals(object? obj)
    {
        return obj is RowData data &&
               _attributeValues.SequenceEqual(data._attributeValues);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_attributeValues);
    }
}
