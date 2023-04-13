using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using System.Reflection;

namespace Janus.Lenses.Implementations;

/// <summary>
/// Describes a lens between a TabularData and IEnumerable of a generic DTO.
/// The DTO must have getters and setters; DTO fields must be named in PascalCase with prefixed underscores.
/// </summary>
/// <typeparam name="TDto">DTO type</typeparam>
public sealed class TabularDataDtoLens<TDto>
    : Lens<TabularData, IEnumerable<TDto>>,
    ICreatingRightLens<IEnumerable<TDto>>,
    ICreatingLeftSpecsLens<TabularData, Type>
{
    /// <summary>
    /// Combined RowDataDtoLens used to transform RowData and TDto
    /// </summary>
    private readonly RowDataDtoLens<TDto> _rowDataLens;

    private readonly Option<Type> _originalViewItemType;

    /// <summary>
    /// Prefix for column names of a generated TabularData
    /// </summary>
    private readonly string _columnNamePrefix;
    internal TabularDataDtoLens(string? columnNamePrefix = null, Type? originalViewItemType = null) : base()
    {
        _rowDataLens = RowDataDtoLenses.Construct<TDto>(originalType: originalViewItemType);
        _columnNamePrefix = columnNamePrefix ?? string.Empty;
        _originalViewItemType = Option<Type>.Some(originalViewItemType);
    }

    /// <summary>
    /// Lens PUT function: IEnumerable[TDto] -> TabularData? -> TabularData.
    /// If source TabularData? is not given, its structure is inferred from the TDto type and a new name is given to the TabularData.
    /// </summary>
    public override Func<IEnumerable<TDto>, TabularData?, TabularData> Put =>
        (view, originalSource) => view.Map(viewItem => _rowDataLens.Put(viewItem, _rowDataLens.CreateLeft(view.FirstOrDefault()?.GetType() ?? typeof(TDto))).ColumnValues)
                                      .Aggregate(TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>((originalSource ?? CreateLeft(view.FirstOrDefault()?.GetType() ?? typeof(TDto))).ColumnDataTypes)),
                                                 (acc, rowData) => acc.AddRow(conf => conf.WithRowData(new Dictionary<string, object?>(rowData))))
                                      .WithName((originalSource ?? CreateLeft(view.FirstOrDefault()?.GetType() ?? typeof(TDto))).Name)
                                      .Build();

    /// <summary>
    /// Lens GET function: TabularData -> IEnumerable[TDto]
    /// </summary>
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

    /// <summary>
    /// Determines column data types fro a given DTO system Type
    /// </summary>
    /// <param name="dtoType">DTO type</param>
    /// <returns>Column data types for specifying a TabularData structure</returns>
    private Dictionary<string, DataTypes> DetermineColumnDataTypesForDto(Type? dtoType = null)
    {
        dtoType ??= typeof(TDto);
        return dtoType.GetRuntimeProperties().ToDictionary(p => _columnNamePrefix + p.Name, p => TypeMappings.MapToDataType(p.PropertyType));
    }
}

/// <summary>
/// TabularDataDtoLens extension class
/// </summary>
public static class TabularDataDtoLenses
{
    /// <summary>
    /// Constructs a TabularDataDtoLens
    /// </summary>
    /// <typeparam name="TDto">Type of DTO</typeparam>
    /// <param name="columnNamePrefix">Explicit prefix of column names in a TabularData</param>
    /// <returns>TabularDataDtoLens instance</returns>
    public static TabularDataDtoLens<TDto> Construct<TDto>(string? columnNamePrefix = null, Type? originalType = null)
        => new TabularDataDtoLens<TDto>(columnNamePrefix, originalType);
}
