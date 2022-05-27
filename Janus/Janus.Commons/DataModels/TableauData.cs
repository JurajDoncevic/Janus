using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.DataModels;

public class TableauData
{
    private readonly List<RowData> _rowData;
    private readonly Dictionary<string, DataTypes> _attributeDataTypes;

    internal TableauData(Dictionary<string, DataTypes> attributeDataTypes)
    {
        _attributeDataTypes = attributeDataTypes;
        _rowData = new List<RowData>();
    }

    [JsonConstructor]
    internal TableauData(IEnumerable<RowData> rowData, Dictionary<string, DataTypes> attributeDataTypes)
    {
        _rowData = rowData.ToList();
        _attributeDataTypes = attributeDataTypes;
    }

    internal void AddRow(RowData rowData)
    {
        _rowData.Add(rowData);
    }

    public IReadOnlyList<RowData> RowData => _rowData;

    public IReadOnlyDictionary<string, DataTypes> AttributeDataTypes => _attributeDataTypes;

    public RowData this[int index] => _rowData[index];

    public override string ToString()
        => string.Join("\n", _rowData);
}
