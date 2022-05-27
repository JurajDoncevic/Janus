using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.DataModels;

public class RowData
{
    private readonly Dictionary<string, object> _attributeValues;

    [JsonConstructor]
    internal RowData(Dictionary<string, object> attributeValues!!)
    {
        _attributeValues = attributeValues;
    }

    public IReadOnlyDictionary<string, object> AttributeValues => _attributeValues;

    public object this[string attributeId] => _attributeValues[attributeId];

    public override string ToString()
        => "(" + string.Join(";", _attributeValues.Values) + ")";
}
