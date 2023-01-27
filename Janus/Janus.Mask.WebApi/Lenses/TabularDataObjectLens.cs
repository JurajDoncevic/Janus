﻿using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using System.Reflection;

namespace Janus.Mask.WebApi.Lenses;

public class TabularDataObjectLens<TView>
{
    private readonly string _columnNamePrefix;
    public TabularDataObjectLens(string? columnNamePrefix = null)
    {
        _columnNamePrefix = columnNamePrefix ?? string.Empty;
    }

    public Func<TabularData, IEnumerable<TView>> Get => _get;
    public Func<IEnumerable<TView>, TabularData> Put => _put.Apply(_columnNamePrefix);

    // maybe return a monad containing name mappings? - bidirectional lens for field names na tabular column names
    private readonly Func<TabularData, IEnumerable<TView>> _get =
        (source) =>
        {
            var viewType = typeof(TView);

            var viewItems = Enumerable.Empty<TView>();

            foreach (var rowData in source.RowData)
            {
                var viewItem = Activator.CreateInstance<TView>();
                foreach (var (colName, value) in rowData.ColumnValues.Map(t => (t.Key.Split('.').Last(), t.Value)))
                {
                    string fieldName = $"_{colName}";//$"_{char.ToLower(colName.First())}{string.Concat(colName.Skip(1))}";

                    var targetField = viewType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                    targetField?.SetValue(viewItem, value);
                }
                viewItems = viewItems.Append(viewItem);
            }

            return viewItems;
        };

    private readonly Func<string, IEnumerable<TView>, TabularData> _put =
        (columnNamePrefix, view) =>
        {
            var viewType = typeof(TView);

            var columnInfos =
                viewType.GetFields()
                .Map(field => (name: field.Name, type: field.FieldType))
                .Map(t => (fieldName: t.name, fieldType: t.type, columnName: $"{columnNamePrefix}{char.ToUpper(t.name.Skip(1).First())}{string.Concat(t.name.Skip(2))}"))
                .ToDictionary(t => t.fieldName, t => t);

            var columnDataTypes =
                columnInfos.ToDictionary(t => t.Value.columnName, t => TypeMappings.MapToDataType(t.Value.fieldType));

            var tblrBuilder = TabularDataBuilder.InitTabularData(columnDataTypes);

            foreach (var viewItem in view)
            {
                var rowData = new Dictionary<string, object?>();

                foreach (var field in viewType.GetFields())
                {
                    var value = field.GetValue(viewItem);
                    var columnName = columnInfos[field.Name].columnName;
                    rowData.Add(columnName, value);
                }

                tblrBuilder = tblrBuilder.AddRow(conf => conf.WithRowData(rowData));
            }

            var data = tblrBuilder.Build();

            return data;
        };

}

