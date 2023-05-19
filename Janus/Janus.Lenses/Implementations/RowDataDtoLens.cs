using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using System.Reflection;

namespace Janus.Lenses.Implementations;
public sealed class RowDataDtoLens<TDto> 
    : SymmetricLens<RowData, TDto>
    where TDto : class
{
    private readonly Option<Type> _dtoType;
    private readonly Option<string> _columnNamePrefix;


    internal RowDataDtoLens(string? columnNamePrefix = null, Type? dtoType = null) : base()
    {
        _columnNamePrefix = Option<string>.Some(columnNamePrefix); // evaluates to some or none
        _dtoType = Option<Type>.Some(dtoType); // evaluates to None if null
    }

    protected override Result<RowData> _CreateLeft(TDto? right)
        => Results.AsResult(() =>
            right is null
            ? RowData.FromDictionary(
                typeof(TDto).GetRuntimeProperties().ToDictionary(p => p.Name, p => GetDefaultValue(TypeMappings.MapToDataType(p.PropertyType)))
            )
            : RowData.FromDictionary(
                (_dtoType.Value ?? right?.GetType() ?? typeof(TDto)).GetRuntimeProperties().ToDictionary(p => p.Name, p => p.GetValue(right))
            )
        );

    protected override Result<TDto> _CreateRight(RowData? left)
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        => Results.AsResult(() =>
        {
            Type rightType = _dtoType.Value ?? typeof(TDto);

            var rightItem = Activator.CreateInstance(rightType);// Activator.CreateInstance<TDto>();
            foreach (var (colName, value) in left?.ColumnValues.Map(t => (t.Key.Split('.').Last(), t.Value)) ?? Enumerable.Empty<(string, object?)>())
            {
                string fieldName = $"_{colName}";

                var targetField = rightType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                targetField?.SetValue(rightItem, value);
            }

            return (TDto)rightItem;
        });
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

    protected override Result<RowData> _PutLeft(TDto right, RowData? left)
        => Results.AsResult(() =>
        {
            var dtoType = right?.GetType() ?? _dtoType.Value ?? typeof(TDto);

            string columnNamePrefix =
                _columnNamePrefix
                ? _columnNamePrefix.Value
                : FindLongestCommonPrefix(
                    (left ?? CreateLeft(null).Match(
                        l => l, 
                        msg => RowData.FromDictionary(new Dictionary<string, object?>()))
                    ).ColumnValues.Keys
                    );

            var columnInfos =
                dtoType.GetRuntimeProperties()
                .Map(field => (name: field.Name, type: field.PropertyType))
                .Map(t => (fieldName: t.name, fieldType: t.type, columnName: $"{columnNamePrefix}{t.name}"))
                .ToDictionary(t => t.fieldName, t => t);

            var columnDataTypes =
                columnInfos.ToDictionary(t => t.Value.columnName, t => TypeMappings.MapToDataType(t.Value.fieldType));

            var rowData = new Dictionary<string, object?>(
                (left ?? CreateLeft(null).Match(
                        l => l,
                        msg => RowData.FromDictionary(new Dictionary<string, object?>())
                        )
                    ).ColumnValues
                );

            foreach (var property in dtoType.GetRuntimeProperties())
            {
                var value = property.GetValue(right);
                var columnName = columnInfos[property.Name].columnName;
                rowData[columnName] = value;
            }

            return RowData.FromDictionary(rowData);
        });

    protected override Result<TDto> _PutRight(RowData left, TDto? right)
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        => Results.AsResult(() =>
        {
            Type rightType = _dtoType.Value ?? typeof(TDto);

            var rightItem = Activator.CreateInstance(rightType);// Activator.CreateInstance<TDto>();
            foreach (var (colName, value) in left?.ColumnValues.Map(t => (t.Key.Split('.').Last(), t.Value)) ?? Enumerable.Empty<(string, object?)>())
            {
                string fieldName = $"_{colName}";

                var targetField = rightType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                targetField?.SetValue(rightItem, value);
            }

            return (TDto)rightItem;
        });
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

    #region HELPER METHODS
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

    #endregion
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
    public static RowDataDtoLens<TDto> Construct<TDto>(string? columnNamePrefix = null, Type? dtoType = null) where TDto : class
        => new RowDataDtoLens<TDto>(columnNamePrefix, dtoType);
}
