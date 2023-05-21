using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using System.Reflection;

namespace Janus.Lenses.Implementations;
public class TabularDataDtoLens<TDto> 
    : SymmetricLens<TabularData, IEnumerable<TDto>>
    where TDto : class
{
    /// <summary>
    /// Combined RowDataDtoLens used to transform RowData and TDto - this is what COMBINATORS are used for
    /// </summary>
    private readonly RowDataDtoLens<TDto> _rowDataLens;

    private readonly Option<Type> _dtoType;

    /// <summary>
    /// Prefix for column names of a generated TabularData
    /// </summary>
    private readonly string _columnNamePrefix;
    internal TabularDataDtoLens(string? columnNamePrefix = null, Type? dtoType = null) : base()
    {
        _columnNamePrefix = columnNamePrefix ?? string.Empty;
        _rowDataLens = SymmetricRowDataDtoLenses.Construct<TDto>(_columnNamePrefix, dtoType);
        _dtoType = Option<Type>.Some(dtoType);
    }

    protected override Result<TabularData> _CreateLeft(Option<IEnumerable<TDto>> right)
        => Results.AsResult(() =>
        {
            var dtoType = right.Value?.FirstOrDefault()?.GetType() ?? typeof(TDto);
            var dtoValues = right.Value ?? Enumerable.Empty<TDto>();
            var columnDataTypes = DetermineColumnDataTypesForDto(dtoType);
            var tabularData =
                dtoValues.Fold(
                    TabularDataBuilder.InitTabularData(columnDataTypes),
                    (dtoValue, tabularBuilder)
                        => tabularBuilder.AddRow(conf => conf.WithRowData(new Dictionary<string, object?>(
                            _rowDataLens.CreateLeft(Option<TDto>.Some(dtoValue)).Match(
                                row => row,
                                msg => _rowDataLens.CreateLeft(Option<TDto>.None).Data // doomed of this breaks
                                ).ColumnValues
                            ))
                        ))
                .Build();
            return tabularData;
        });

    protected override Result<IEnumerable<TDto>> _CreateRight(Option<TabularData> left)
        => Results.AsResult(() =>
        {
            var right =
                left.IsSome
                ? left.Value.RowData
                    .Map(rd => _rowDataLens.PutRight(rd, Option<TDto>.None))
                    .Fold(Results.OnSuccess<IEnumerable<TDto>>(Enumerable.Empty<TDto>()),
                         (dtoRes, results) => results.Bind(r => dtoRes.Map(dto => r.Append(dto))))
                : Results.OnSuccess(Enumerable.Empty<TDto>());
            return right;
        });

    protected override Result<TabularData> _PutLeft(IEnumerable<TDto> right, Option<TabularData> left)
        => Results.AsResult(() =>
        {
            var tabularData =
            right.Map(rightItem => _rowDataLens.CreateLeft(Option<TDto>.Some(rightItem))
                                    .Bind(createdLeft => _rowDataLens.PutLeft(rightItem, Option<RowData>.Some(createdLeft)))
                                    .Match(l => l.ColumnValues, msg => new Dictionary<string, object?>())
                                    )
                 .Fold(TabularDataBuilder.InitTabularData(new Dictionary<string, DataTypes>((left.Value ?? CreateLeft(Option<IEnumerable<TDto>>.Some(right)).Data).ColumnDataTypes)),
                   (values, tabularBuilder) => tabularBuilder.AddRow(conf => conf.WithRowData(new Dictionary<string, object?>(values)))
                 )
                 .WithName(left.Value?.Name ?? CreateLeft(Option<IEnumerable<TDto>>.None).Data.Name)
                 .Build();
            return tabularData;
        });

    protected override Result<IEnumerable<TDto>> _PutRight(TabularData left, Option<IEnumerable<TDto>> right)
        => Results.AsResult(() =>
        {
            return left.RowData.Map(rd => _rowDataLens.PutRight(rd, Option<TDto>.None))
                .Fold(Results.OnSuccess<IEnumerable<TDto>>(Enumerable.Empty<TDto>()),
                     (dtoRes, results) => results.Bind(r => dtoRes.Map(dto => r.Append(dto))));
        });

    #region HELPER METHODS
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
    #endregion
}

/// <summary>
/// TabularDataDtoLens extension class
/// </summary>
public static class SymmetricTabularDataDtoLenses
{
    /// <summary>
    /// Constructs a TabularDataDtoLens
    /// </summary>
    /// <typeparam name="TDto">Type of DTO</typeparam>
    /// <param name="columnNamePrefix">Explicit prefix of column names in a TabularData</param>
    /// <returns>TabularDataDtoLens instance</returns>
    public static TabularDataDtoLens<TDto> Construct<TDto>(string? columnNamePrefix = null, Type? dtoType = null) where TDto : class
        => new TabularDataDtoLens<TDto>(columnNamePrefix, dtoType);
}