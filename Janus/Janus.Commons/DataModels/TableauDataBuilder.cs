using Janus.Commons.DataModels.Exceptions;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.DataModels;

public interface IPostInitTableauDataBuilder
{
    IPostInitTableauDataBuilder AddRow(Func<RowDataBuilder, RowDataBuilder> configuration);
    TableauData Build();
}

public class TableauDataBuilder : IPostInitTableauDataBuilder
{
    private readonly Dictionary<string, DataTypes> _attributeDataTypes;
    private readonly List<RowData> _rows;

    private TableauDataBuilder(Dictionary<string, DataTypes> attributeDataTypes!!)
    {
        _attributeDataTypes = attributeDataTypes;
        _rows = new List<RowData>();
    }

    public static IPostInitTableauDataBuilder InitTableauData(Dictionary<string, DataTypes> attributeDataTypes!!)
    {
        return new TableauDataBuilder(attributeDataTypes);
    }

    public IPostInitTableauDataBuilder AddRow(Func<RowDataBuilder, RowDataBuilder> configuration)
    {
        var rowDataBuilder = RowDataBuilder.InitRowWithDataTypes(_attributeDataTypes);
        var row = configuration(rowDataBuilder).Build();
        _rows.Add(row);

        return this;
    }

    public TableauData Build() => new TableauData(_rows, _attributeDataTypes);
}

public class RowDataBuilder
{
    private readonly Dictionary<string, DataTypes> _attributeDataTypes;
    private Dictionary<string, object>? _attributeValues;

    internal RowDataBuilder(Dictionary<string, DataTypes> attributeDataTypes!!)
    {
        _attributeDataTypes = attributeDataTypes;
        _attributeValues = null;
    }

    public static RowDataBuilder InitRowWithDataTypes(Dictionary<string, DataTypes> attributeDataTypes!!)
    {
        return new RowDataBuilder(attributeDataTypes);
    }

    public RowDataBuilder WithRowData(Dictionary<string, object> attributeValues)
    {
        if (_attributeDataTypes.Count != attributeValues.Count
            || !attributeValues.Keys.All(_attributeDataTypes.ContainsKey))
            throw new IncompatibleRowDataTypeException(attributeValues.Keys.ToList(), _attributeDataTypes.Keys.ToList());

        //var types = attributeValues.Values.Select(v => v.GetType()).ToList();

        var attrValue = attributeValues.Where(kvp => 
                                            kvp.Value !=null &&
                                            !kvp.Value.GetType().IsEquivalentTo(TypeMappings.MapToType(_attributeDataTypes[kvp.Key])))
                                       .Select(kvp => (attrId: kvp.Key, attrValue: kvp.Value))
                                       .FirstOrDefault();
        if (attrValue != default)
            throw new IncompatibleDotNetTypeException(attrValue.attrId, attrValue.GetType());
        _attributeValues = attributeValues;

        return this;
    }

    internal RowData Build()
        => _attributeValues == null && _attributeDataTypes.Count != 0
           ? throw new IncompatibleRowDataTypeException(new List<string>(), _attributeDataTypes.Keys.ToList())
           : new RowData(_attributeValues ?? new Dictionary<string, object>());
}