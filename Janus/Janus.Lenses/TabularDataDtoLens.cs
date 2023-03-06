using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;

namespace Janus.Lenses;
public class TabularDataDtoLens<TDto> 
    : Lens<TabularData, IEnumerable<TDto>>,
    ICreatingRightLens<TDto>,
    ICreatingLeftSpecsLens<TabularData, Dictionary<string, DataTypes>>
{
    private readonly RowDataDtoLens<TDto> _rowDataLens;

    internal TabularDataDtoLens() : base()
    {
        _rowDataLens = RowDataDtoLens.Construct<TDto>();
    }

    public override Func<IEnumerable<TDto>, TabularData?, TabularData> Put =>
        (view, originalSource) => view.Map(viewItem => _rowDataLens.Put(viewItem, (originalSource ?? CreateLeft()).RowData.First()))
                                      .Aggregate(TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>((originalSource ?? CreateLeft()).ColumnDataTypes)),
                                                 (acc, rowData) => acc.AddRow(conf => conf.WithRowData(new Dictionary<string, object?>(rowData.ColumnValues))))
                                      .WithName((originalSource ?? CreateLeft()).Name)
                                      .Build();

    public override Func<TabularData, IEnumerable<TDto>> Get =>
        (source) => source.RowData.Map(rd => _rowDataLens.Get(rd));


    public TabularData CreateLeft(Dictionary<string, DataTypes>? columnDataTypes = null)
    {
        var tabularData = 
            TabularDataBuilder.InitTabularData(columnDataTypes ?? DetermineColumnDataTypes())
            .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>(_rowDataLens.CreateLeft(columnDataTypes).ColumnValues)))
            .Build();
        return tabularData;
    }

    public TDto CreateRight()
    {
        return (TDto)Activator.CreateInstance(typeof(TDto))!;
    }

    private Dictionary<string, DataTypes> DetermineColumnDataTypes()
    {
        var dtoType = Activator.CreateInstance<TDto>()?.GetType() ?? typeof(TDto);
        return dtoType.GetProperties().ToDictionary(p => p.Name, p => TypeMappings.MapToDataType(p.PropertyType));
    }
}

public static class TabularDataDtoLens
{
    public static TabularDataDtoLens<T> Construct<T>()
        => new TabularDataDtoLens<T>();
}
