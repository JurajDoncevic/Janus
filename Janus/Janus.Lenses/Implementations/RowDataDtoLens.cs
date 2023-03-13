using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using System.Reflection;

namespace Janus.Lenses.Implementations;

/// <summary>
/// Describes a lens between a TabularData RowData and a generic DTO.
/// The DTO must have getters and setters; DTO fields must be named in PascalCase with prefixed underscores.
/// </summary>
/// <typeparam name="TDto">DTO type</typeparam>
public sealed class RowDataDtoLens<TDto>
    : Lens<RowData, TDto>,
    ICreatingLeftSpecsLens<RowData, Type>
{
    /// <summary>
    /// Prefix for column names when generating tabular data
    /// </summary>
    private readonly Option<string> _columnNamePrefix;

    private readonly Option<Type> _originalType;

    internal RowDataDtoLens(string? columnNamePrefix = null, Type? originalType = null) : base()
    {
        _columnNamePrefix = Option<string>.Some(columnNamePrefix); // evaluates to some or none
        _originalType = Option<Type>.Some(originalType);
    }

    /// <summary>
    /// Lens PUT function: TDto -> RowData? -> RowData.
    /// If source RowData? is not given, its structure is inferred from the TDto type.
    /// </summary>
    public override Func<TDto, RowData?, RowData> Put =>
        (view, originalSource) =>
        {
            var dtoType = view?.GetType() ?? _originalType.Value ?? typeof(TDto);

            string columnNamePrefix =
                _columnNamePrefix
                ? _columnNamePrefix.Value
                : FindLongestCommonPrefix((originalSource ?? CreateLeft(dtoType)).ColumnValues.Keys);

            var columnInfos =
                dtoType.GetRuntimeProperties()
                .Map(field => (name: field.Name, type: field.PropertyType))
                .Map(t => (fieldName: t.name, fieldType: t.type, columnName: $"{columnNamePrefix}{t.name}"))
                .ToDictionary(t => t.fieldName, t => t);

            var columnDataTypes =
                columnInfos.ToDictionary(t => t.Value.columnName, t => TypeMappings.MapToDataType(t.Value.fieldType));

            var rowData = new Dictionary<string, object?>((originalSource ?? CreateLeft(dtoType)).ColumnValues);

            foreach (var property in dtoType.GetRuntimeProperties())
            {
                var value = property.GetValue(view);
                var columnName = columnInfos[property.Name].columnName;
                rowData[columnName] = value;
            }

            return RowData.FromDictionary(rowData);
        };

    /// <summary>
    /// Lens GET function: RowData -> TDto
    /// </summary>
    public override Func<RowData, TDto> Get =>
        (source) =>
        {
            var viewType = _originalType.Value ?? typeof(TDto);


            var viewItem = Activator.CreateInstance(viewType);// Activator.CreateInstance<TDto>();
            foreach (var (colName, value) in source.ColumnValues.Map(t => (t.Key.Split('.').Last(), t.Value)))
            {
                string fieldName = $"_{colName}";

                var targetField = viewType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                targetField?.SetValue(viewItem, value);
            }

            return (TDto)viewItem;
        };

    public RowData CreateLeft(Type dtoType)
        => RowData.FromDictionary(
            dtoType.GetRuntimeProperties().ToDictionary(p => p.Name, p => GetDefaultValue(TypeMappings.MapToDataType(p.PropertyType)))
            );

    /// <summary>
    /// Gets the default system value for the data type
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns>Default value boxed as a object?</returns>
    private object? GetDefaultValue(DataTypes dataType)
        => dataType switch
        {
            DataTypes.INT => 0,
            DataTypes.LONGINT => 0L,
            DataTypes.DECIMAL => 0.0,
            DataTypes.STRING => string.Empty,
            DataTypes.DATETIME => DateTime.MinValue,
            DataTypes.BOOLEAN => false,
            DataTypes.BINARY => new byte[0] { },
            _ => null
        };

    /// <summary>
    /// Finds the longest prefix in a IEnumerable of strings
    /// </summary>
    /// <param name="strings"></param>
    /// <returns>Longest prefix in all strings</returns>
    private string FindLongestCommonPrefix(IEnumerable<string> strings)
    {
        if (strings == null || strings.Count() == 0)
        {
            return string.Empty;
        }

        string prefix = strings.First();

        // Iterate through all strings in the list and find the longest common prefix
        for (int i = 1; i < strings.Count(); i++)
        {
            string currentString = strings.ElementAt(i);
            int j = 0;
            while (j < prefix.Length && j < currentString.Length && prefix[j] == currentString[j])
            {
                j++;
            }
            prefix = prefix.Substring(0, j);
        }

        return prefix;
    }
}

/// <summary>
/// RowDataDtoLens extension class
/// </summary>
public static class RowDataDtoLenses
{
    /// <summary>
    /// Constructs a RowDataDtoLens
    /// </summary>
    /// <typeparam name="TDto">Type of DTO</typeparam>
    /// <param name="columnNamePrefix">Explicit prefix of column names in a RowData</param>
    /// <returns>RowDataDtoLens instance</returns>
    public static RowDataDtoLens<TDto> Construct<TDto>(string? columnNamePrefix = null, Type? originalType = null)
        => new RowDataDtoLens<TDto>(columnNamePrefix, originalType);
}
