using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.DataModels;

/// <summary>
/// Describes a row of tableau data
/// </summary>
public class RowData
{
    private readonly Dictionary<string, object> _attributeValues;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attributeValues">Values of attributes in the row</param>
    [JsonConstructor]
    internal RowData(Dictionary<string, object> attributeValues!!)
    {
        _attributeValues = attributeValues;
    }

    /// <summary>
    /// Values of attributes in the row
    /// </summary>
    public IReadOnlyDictionary<string, object> AttributeValues => _attributeValues;

    public object this[string attributeId] => _attributeValues[attributeId];

    public override string ToString()
        => "(" + string.Join(";", _attributeValues.Values) + ")";
}
