using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.DataModels;

public class RowData
{
    private readonly Dictionary<string, DataTypes> _attributeDataTypes;
    private readonly Dictionary<string, object> _attributeValues;

    public RowData(Dictionary<string, DataTypes> attributeDataTypes!!, Dictionary<string, object> attributeValues!!)
    {
        _attributeDataTypes = attributeDataTypes;
        _attributeValues = attributeValues;
    }

    public IReadOnlyDictionary<string, DataTypes> AttributeDataTypes => _attributeDataTypes;

    public IReadOnlyDictionary<string, object> AttributeValues => _attributeValues;

    public override string ToString()
        => "(" + string.Join(";", _attributeValues.Values) + ")";
}
