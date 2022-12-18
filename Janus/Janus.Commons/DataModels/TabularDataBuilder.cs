using Janus.Commons.DataModels.Exceptions;
using Janus.Commons.SchemaModels;

namespace Janus.Commons.DataModels;

public interface IPostInitTabularDataBuilder
{
    /// <summary>
    /// Sets the tabular data's ID
    /// </summary>
    /// <param name="tabularDataName"></param>
    /// <returns>Current builder</returns>
    IPostInitTabularDataBuilder WithName(string tabularDataName);

    /// <summary>
    /// Adds the configured row of data to the tabular data
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns>Current builder</returns>
    IPostInitTabularDataBuilder AddRow(Func<RowDataBuilder, RowDataBuilder> configuration);

    /// <summary>
    /// Builds the configured tabular data
    /// </summary>
    /// <returns></returns>
    TabularData Build();
}

/// <summary>
/// Builder for tabular data
/// </summary>
public sealed class TabularDataBuilder : IPostInitTabularDataBuilder
{
    private readonly Dictionary<string, DataTypes> _columnDataTypes;
    private readonly List<RowData> _rows;
    private string? _name = null;

    private TabularDataBuilder(Dictionary<string, DataTypes> columnDataTypes)
    {
        _columnDataTypes = columnDataTypes ?? throw new ArgumentNullException(nameof(columnDataTypes));
        _rows = new List<RowData>();
    }

    public static IPostInitTabularDataBuilder InitTabularData(Dictionary<string, DataTypes> columnDataTypes)
    {
        if (columnDataTypes is null)
        {
            throw new ArgumentNullException(nameof(columnDataTypes));
        }

        return new TabularDataBuilder(columnDataTypes);
    }

    public IPostInitTabularDataBuilder WithName(string tabularDataName)
    {
        _name = tabularDataName;
        return this;
    }

    public IPostInitTabularDataBuilder AddRow(Func<RowDataBuilder, RowDataBuilder> configuration)
    {
        var rowDataBuilder = RowDataBuilder.InitRowWithDataTypes(_columnDataTypes);
        var row = configuration(rowDataBuilder).Build();
        _rows.Add(row);

        return this;
    }

    public TabularData Build() => new TabularData(_rows, _columnDataTypes, _name);
}

public class RowDataBuilder
{
    private readonly Dictionary<string, DataTypes> _columnDataTypes;
    private Dictionary<string, object?>? _columnValues;

    internal RowDataBuilder(Dictionary<string, DataTypes> columnDataTypes)
    {
        _columnDataTypes = columnDataTypes ?? throw new ArgumentNullException(nameof(columnDataTypes));
        _columnValues = null;
    }

    public static RowDataBuilder InitRowWithDataTypes(Dictionary<string, DataTypes> columnDataTypes)
    {
        if (columnDataTypes is null)
        {
            throw new ArgumentNullException(nameof(columnDataTypes));
        }

        return new RowDataBuilder(columnDataTypes);
    }

    public RowDataBuilder WithRowData(Dictionary<string, object?> columnValues)
    {
        if (_columnDataTypes.Count != columnValues.Count
            || !columnValues.Keys.All(_columnDataTypes.ContainsKey))
            throw new IncompatibleRowDataTypeException(columnValues.Keys.ToList(), _columnDataTypes.Keys.ToList());

        var colValue = columnValues.Where(kvp =>
                                            kvp.Value != null &&
                                            !TypeMappings.IsRepresentableAs(kvp.Value.GetType(), _columnDataTypes[kvp.Key]))
                                       .Select(kvp => (attrId: kvp.Key, attrValue: kvp.Value))
                                       .FirstOrDefault();
        if (colValue != default)
            throw new IncompatibleDotNetTypeException(colValue.attrId, colValue.attrValue?.GetType());
        _columnValues = columnValues;

        return this;
    }

    internal RowData Build()
        => _columnValues == null && _columnDataTypes.Count != 0
           ? throw new IncompatibleRowDataTypeException(new List<string>(), _columnDataTypes.Keys.ToList())
           : new RowData(_columnValues ?? new Dictionary<string, object?>());
}