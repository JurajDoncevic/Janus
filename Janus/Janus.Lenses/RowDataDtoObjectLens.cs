using Janus.Commons.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Lenses;
public class RowDataDtoObjectLens<TView> : Lens<RowData, TView>
{
    internal RowDataDtoObjectLens() : base()
    {
    }

    public override Func<TView, RowData, RowData> Put =>
        (view, originalSource) =>
        {
            var viewType = view?.GetType() ?? typeof(TView);

            var columnInfos =
                viewType.GetRuntimeProperties()
                .Map(field => (name: field.Name, type: field.PropertyType))
                .Map(t => (fieldName: t.name, fieldType: t.type, columnName: $"{columnNamePrefix}{t.name}"))
                .ToDictionary(t => t.fieldName, t => t);

            var columnDataTypes =
                columnInfos.ToDictionary(t => t.Value.columnName, t => TypeMappings.MapToDataType(t.Value.fieldType));

            var tblrBuilder = TabularDataBuilder.InitTabularData(columnDataTypes);

            var rowData = new Dictionary<string, object?>();

            foreach (var property in viewType.GetRuntimeProperties())
            {
                var value = property.GetValue(view);
                var columnName = columnInfos[property.Name].columnName;
                rowData.Add(columnName, value);
            }

            return RowData.FromDictionary(rowData);
        };

    public override Func<RowData, TView> Get =>
        (source) =>
        {
            var viewType = typeof(TView);


            var viewItem = Activator.CreateInstance<TView>();
            foreach (var (colName, value) in source.ColumnValues.Map(t => (t.Key.Split('.').Last(), t.Value)))
            {
                string fieldName = $"_{colName}";

                var targetField = viewType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                targetField?.SetValue(viewItem, value);
            }

            return viewItem;
        };
}
