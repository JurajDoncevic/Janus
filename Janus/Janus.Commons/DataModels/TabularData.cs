using Janus.Commons.DataModels.JsonConversion;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.DataModels;

/// <summary>
/// Describes tabular data
/// </summary>
[JsonConverter(typeof(TabularDataJsonConverter))]
public class TabularData
{
    private readonly List<RowData> _rowData;
    private readonly Dictionary<string, DataTypes> _attributeDataTypes;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="attributeDataTypes">Data types for attributes</param>
    internal TabularData(Dictionary<string, DataTypes> attributeDataTypes)
    {
        _attributeDataTypes = attributeDataTypes;
        _rowData = new List<RowData>();
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rowData">Rows of data to be placed into tabular data</param>
    /// <param name="attributeDataTypes">Data types for attributes</param>
    internal TabularData(IEnumerable<RowData> rowData, Dictionary<string, DataTypes> attributeDataTypes)
    {
        _rowData = rowData.ToList();
        _attributeDataTypes = attributeDataTypes;
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
    public IReadOnlyDictionary<string, DataTypes> AttributeDataTypes => _attributeDataTypes;

    public RowData this[int index] => _rowData[index];

    /// <summary>
    /// Attribute names in the tabular
    /// </summary>
    public HashSet<string> AttributeNames => _attributeDataTypes.Keys.ToHashSet();

    public override string ToString()
        => string.Join("\n", _rowData);
}
