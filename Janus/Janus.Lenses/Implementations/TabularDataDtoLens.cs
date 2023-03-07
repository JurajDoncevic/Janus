using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using System.Reflection;

namespace Janus.Lenses.Implementations;
public class TabularDataDtoLens<TDto>
    : Lens<TabularData, IEnumerable<TDto>>,
    ICreatingRightLens<IEnumerable<TDto>>,
    ICreatingLeftSpecsLens<TabularData, Type>
{
    private readonly RowDataDtoLens<TDto> _rowDataLens;
    private readonly string _columnNamePrefix;
    internal TabularDataDtoLens(string? columnNamePrefix = null) : base()
    {
        _rowDataLens = RowDataDtoLens.Construct<TDto>();
        _columnNamePrefix = columnNamePrefix ?? string.Empty;
    }

    public override Func<IEnumerable<TDto>, TabularData?, TabularData> Put =>
        (view, originalSource) => view.Map(viewItem => _rowDataLens.Put(viewItem, _rowDataLens.CreateLeft(view.FirstOrDefault()?.GetType() ?? typeof(TDto))).ColumnValues)
                                      .Aggregate(TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>((originalSource ?? CreateLeft(view.FirstOrDefault()?.GetType() ?? typeof(TDto))).ColumnDataTypes)),
                                                 (acc, rowData) => acc.AddRow(conf => conf.WithRowData(new Dictionary<string, object?>(rowData))))
                                      .WithName((originalSource ?? CreateLeft(view.FirstOrDefault()?.GetType() ?? typeof(TDto))).Name)
                                      .Build();

    public override Func<TabularData, IEnumerable<TDto>> Get =>
        (source) => source.RowData.Map(rd => _rowDataLens.Get(rd));


    public TabularData CreateLeft(Type dtoType)
    {
        var columnDataTypes = DetermineColumnDataTypesForDto(dtoType);
        var tabularData =
            TabularDataBuilder.InitTabularData(columnDataTypes)
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>(_rowDataLens.CreateLeft(dtoType).ColumnValues)))
            .Build();
        return tabularData;
    }

    public IEnumerable<TDto> CreateRight()
    {
        return Enumerable.Empty<TDto>();
    }

    private Dictionary<string, DataTypes> DetermineColumnDataTypesForDto(Type? dtoType = null)
    {
        dtoType ??= typeof(TDto);
        return dtoType.GetRuntimeProperties().ToDictionary(p => _columnNamePrefix + p.Name, p => TypeMappings.MapToDataType(p.PropertyType));
    }
}

public static class TabularDataDtoLens
{
    public static TabularDataDtoLens<T> Construct<T>(string? columnNamePrefix = null)
        => new TabularDataDtoLens<T>(columnNamePrefix);
}
