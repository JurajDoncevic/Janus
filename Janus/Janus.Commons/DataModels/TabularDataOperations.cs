namespace Janus.Commons.DataModels;
public static class TabularDataOperations
{
    public static Result<TabularData> EquiJoinTabularData(TabularData foreignKeyData, TabularData primaryKeyData, string foreignKeyColumn, string primaryKeyColumn)
        => Results.AsResult(() =>
        {
            // it is expected that all the columns are named differently
            var joinedAttributeTypes = foreignKeyData.ColumnDataTypes.Union(primaryKeyData.ColumnDataTypes).ToDictionary(kv => kv.Key, kv => kv.Value);
            var tabularDataBuilder = TabularDataBuilder.InitTabularData(joinedAttributeTypes);

            foreach (var fkRow in foreignKeyData.RowData)
            {
                
                var pkRow =
                    primaryKeyData.RowData.FirstOrDefault(row => row[primaryKeyColumn].Equals(fkRow[foreignKeyColumn]))?.AttributeValues
                    ?? primaryKeyData.ColumnDataTypes.Keys.ToDictionary(k => k, k => (object?)null);

                var joinedRowValues = fkRow.AttributeValues.Union(pkRow).ToDictionary(kv => kv.Key, kv => kv.Value);

                tabularDataBuilder.AddRow(conf => conf.WithRowData(joinedRowValues));

            }

            return tabularDataBuilder.Build();
        });

    public static Result<TabularData> ProjectTabularDataColumns(TabularData tabularData, HashSet<string> projectionColumnNames)
        => Results.AsResult(() =>
        {
            // are there non-existing columns named in projection?
            if (!projectionColumnNames.IsSubsetOf(tabularData.ColumnNames))
            {
                return Results.OnFailure<TabularData>($"Some column names not found in given tabular data: {string.Join(',', projectionColumnNames.Except(tabularData.ColumnNames))}");
            }

            var projectedDataTypes =
                projectionColumnNames.Map(columnName => (columnName, dataType: tabularData.ColumnDataTypes[columnName]))
                                     .ToDictionary(t => t.columnName, t => t.dataType);

            var tabularDataBuilder = 
                TabularDataBuilder.InitTabularData(projectedDataTypes)
                                  .WithName(tabularData.Name);

            foreach(var rowData in tabularData.RowData)
            {
                // remove non-projected data from a row...
                var projectedRow = 
                    rowData.AttributeValues
                           .Where(kv => projectionColumnNames.Contains(kv.Key))
                           .ToDictionary(kv => kv.Key, kv => kv.Value);
                // ... place the projected row in the builder
                tabularDataBuilder.AddRow(conf => conf.WithRowData(projectedRow));
            }

            return tabularDataBuilder.Build();
        });
}
