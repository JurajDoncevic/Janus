using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using System.Reflection;

namespace Janus.Lenses.Implementations;
public class SymmetricRowDataDtoLens<TDto>
    : SymmetricLens<RowData, TDto>
{
    /// <summary>
    /// Prefix for column names when generating tabular data
    /// </summary>
    private readonly Option<string> _columnNamePrefix;

    private readonly Option<Type> _originalType;

    internal SymmetricRowDataDtoLens(string? columnNamePrefix = null, Type? originalType = null) : base()
    {
        _columnNamePrefix = Option<string>.Some(columnNamePrefix); // evaluates to some or none
        _originalType = Option<Type>.Some(originalType);
    }

    public override Func<RowData, TDto?, TDto> PutRight =>
        (left, right) =>
        {
            var rightType = _originalType.Value ?? typeof(TDto);


            var rightItem = Activator.CreateInstance(rightType);
            foreach (var (colName, value) in left.ColumnValues.Map(t => (t.Key.Split('.').Last(), t.Value)))
            {
                string fieldName = $"_{colName}";

                var targetField = rightType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                targetField?.SetValue(rightItem, value);
            }

            return (TDto)rightItem;
        };

    public override Func<TDto, RowData?, RowData> PutLeft =>
        (right, left) =>
        {
            var dtoType = right?.GetType() ?? _originalType.Value ?? typeof(TDto);

            string columnNamePrefix =
                _columnNamePrefix
                ? _columnNamePrefix.Value
                : FindLongestCommonPrefix((left ?? CreateLeft(dtoType)).ColumnValues.Keys);

            var columnInfos =
                dtoType.GetRuntimeProperties()
                .Map(field => (name: field.Name, type: field.PropertyType))
                .Map(t => (fieldName: t.name, fieldType: t.type, columnName: $"{columnNamePrefix}{t.name}"))
                .ToDictionary(t => t.fieldName, t => t);

            var columnDataTypes =
                columnInfos.ToDictionary(t => t.Value.columnName, t => TypeMappings.MapToDataType(t.Value.fieldType));

            var rowData = new Dictionary<string, object?>((left ?? CreateLeft(dtoType)).ColumnValues);

            foreach (var property in dtoType.GetRuntimeProperties())
            {
                var value = property.GetValue(right);
                var columnName = columnInfos[property.Name].columnName;
                rowData[columnName] = value;
            }

            return RowData.FromDictionary(rowData);
        };

    public override RowData CreateLeft(Option<TDto> optionalDtoValue)
    {
        var dtoType = _originalType.Value ?? optionalDtoValue.Value?.GetType() ?? typeof(TDto);

        string columnNamePrefix =
            _columnNamePrefix
            ? _columnNamePrefix.Value
            : FindLongestCommonPrefix((CreateLeft(dtoType)).ColumnValues.Keys);

        var columnInfos =
            dtoType.GetRuntimeProperties()
            .Map(field => (name: field.Name, type: field.PropertyType))
            .Map(t => (fieldName: t.name, fieldType: t.type, columnName: $"{columnNamePrefix}{t.name}"))
            .ToDictionary(t => t.fieldName, t => t);

        var rowData = new Dictionary<string, object?>((CreateLeft(dtoType)).ColumnValues);

        if (optionalDtoValue)
        {
            var dtoValue = optionalDtoValue.Value;
            foreach (var property in dtoType.GetRuntimeProperties())
            {
                var value = property.GetValue(dtoValue);
                var columnName = columnInfos[property.Name].columnName;
                rowData[columnName] = value;
            }
        }


        return RowData.FromDictionary(rowData);
    }

    public override TDto CreateRight(Option<RowData> optionalRowData)
    {
        var rightType = _originalType.Value ?? typeof(TDto);
        var rightItem = Activator.CreateInstance(rightType);
        if (optionalRowData)
        {
            var rowData = optionalRowData.Value;
            foreach (var (colName, value) in rowData.ColumnValues.Map(t => (t.Key.Split('.').Last(), t.Value)))
            {
                string fieldName = $"_{colName}";

                var targetField = rightType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                targetField?.SetValue(rightItem, value);
            }
        }
        return (TDto)rightItem;
    }

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
public static class SymmetricRowDataDtoLenses
{
    /// <summary>
    /// Constructs a RowDataDtoLens
    /// </summary>
    /// <typeparam name="TDto">Type of DTO</typeparam>
    /// <param name="columnNamePrefix">Explicit prefix of column names in a RowData</param>
    /// <returns>RowDataDtoLens instance</returns>
    public static SymmetricRowDataDtoLens<TDto> Construct<TDto>(string? columnNamePrefix = null, Type? originalType = null)
        => new SymmetricRowDataDtoLens<TDto>(columnNamePrefix, originalType);
}