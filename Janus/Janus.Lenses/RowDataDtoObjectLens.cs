using Janus.Commons.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Lenses;
public sealed class RowDataDtoObjectLens<TDto> : Lens<RowData, TDto>
{
    internal RowDataDtoObjectLens() : base()
    {
    }

    public override Func<TDto, RowData, RowData> Put =>
        (view, originalSource) =>
        {
            var dtoType = view?.GetType() ?? typeof(TDto);

            string columnNamePrefix = FindLongestCommonPrefix(originalSource.ColumnValues.Keys);

            var columnInfos =
                dtoType.GetRuntimeProperties()
                .Map(field => (name: field.Name, type: field.PropertyType))
                .Map(t => (fieldName: t.name, fieldType: t.type, columnName: $"{columnNamePrefix}{t.name}"))
                .ToDictionary(t => t.fieldName, t => t);

            var columnDataTypes =
                columnInfos.ToDictionary(t => t.Value.columnName, t => TypeMappings.MapToDataType(t.Value.fieldType));

            var rowData = new Dictionary<string, object?>(originalSource.ColumnValues);

            foreach (var property in dtoType.GetRuntimeProperties())
            {
                var value = property.GetValue(view);
                var columnName = columnInfos[property.Name].columnName;
                rowData[columnName] = value;
            }

            return RowData.FromDictionary(rowData);
        };

    public override Func<RowData, TDto> Get =>
        (source) =>
        {
            var viewType = typeof(TDto);


            var viewItem = Activator.CreateInstance<TDto>();
            foreach (var (colName, value) in source.ColumnValues.Map(t => (t.Key.Split('.').Last(), t.Value)))
            {
                string fieldName = $"_{colName}";

                var targetField = viewType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                targetField?.SetValue(viewItem, value);
            }

            return viewItem;
        };

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

public static class RowDataDtoObjectLens
{
    public static RowDataDtoObjectLens<T> Create<T>()
        => new RowDataDtoObjectLens<T>();
}
