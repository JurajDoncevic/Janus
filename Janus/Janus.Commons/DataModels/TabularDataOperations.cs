using Janus.Commons.SelectionExpressions;

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
                    primaryKeyData.RowData.FirstOrDefault(row => row[primaryKeyColumn].Equals(fkRow[foreignKeyColumn]))?.ColumnValues
                    ?? primaryKeyData.ColumnDataTypes.Keys.ToDictionary(k => k, k => (object?)null);

                var joinedRowValues = fkRow.ColumnValues.Union(pkRow).ToDictionary(kv => kv.Key, kv => kv.Value);

                tabularDataBuilder = tabularDataBuilder.AddRow(conf => conf.WithRowData(joinedRowValues));

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
                    rowData.ColumnValues
                           .Where(kv => projectionColumnNames.Contains(kv.Key))
                           .ToDictionary(kv => kv.Key, kv => kv.Value);
                // ... place the projected row in the builder
                tabularDataBuilder = tabularDataBuilder.AddRow(conf => conf.WithRowData(projectedRow));
            }

            return tabularDataBuilder.Build();
        });

    public static Result<TabularData> SelectData(TabularData tabularData, SelectionExpression selectionExpression)
        => Results.AsResult(() =>
        {
            var selectionFunc = GenerateSelectionFunc(selectionExpression);

            var tabularDataBuilder = 
                TabularDataBuilder.InitTabularData(tabularData.ColumnDataTypes.ToDictionary(kv => kv.Key, kv => kv.Value))
                                  .WithName(tabularData.Name);

            foreach (var rowData in tabularData.RowData)
            {
                var columnValues = rowData.ColumnValues.ToDictionary(kv => kv.Key, kv=> kv.Value);

                if (selectionFunc(columnValues))
                {
                    tabularDataBuilder = tabularDataBuilder.AddRow(conf => conf.WithRowData(columnValues));
                }
            }

            return tabularDataBuilder.Build();
        });

    private static Func<Dictionary<string, object?>, bool> GenerateSelectionFunc(SelectionExpression selectionExpression)
        => selectionExpression switch
        {
            TrueLiteral trueLiteral => (Dictionary<string, object?> row) => true,
            FalseLiteral falseLiteral => (Dictionary<string, object?> row) => false,
            LesserOrEqualThan lesserOrEqualThan => (Dictionary<string, object?> args) => Convert.ToDouble(args[lesserOrEqualThan.AttributeId.ToString()]) <= Convert.ToDouble(lesserOrEqualThan.Value),
            LesserThan lesserThan => (Dictionary<string, object?> args) => Convert.ToDouble(args[lesserThan.AttributeId.ToString()]) < Convert.ToDouble(lesserThan.Value),
            GreaterOrEqualThan greaterOrEqualThan => (Dictionary<string, object?> args) => Convert.ToDouble(args[greaterOrEqualThan.AttributeId.ToString()]) >= Convert.ToDouble(greaterOrEqualThan.Value),
            GreaterThan greaterThan => (Dictionary<string, object?> args) => Convert.ToDouble(args[greaterThan.AttributeId.ToString()]) > Convert.ToDouble(greaterThan.Value),
            EqualAs eq => (Dictionary<string, object?> row) => row[eq.AttributeId.ToString()]?.Equals(eq.Value) ?? false,
            NotEqualAs neq => (Dictionary<string, object?> row) => row[neq.AttributeId.ToString()]?.Equals(neq.Value) ?? true,
            AndOperator andOperator => (Dictionary<string, object?> row) => GenerateSelectionFunc(andOperator.LeftOperand)(row) && GenerateSelectionFunc(andOperator.RightOperand)(row),
            OrOperator orOperator => (Dictionary<string, object?> row) => GenerateSelectionFunc(orOperator.LeftOperand)(row) || GenerateSelectionFunc(orOperator.RightOperand)(row),
            NotOperator notOperator => (Dictionary<string, object?> row) => GenerateSelectionFunc(notOperator.Operand)(row),
            _ => (Dictionary<string, object?> row) => true
        };
}
