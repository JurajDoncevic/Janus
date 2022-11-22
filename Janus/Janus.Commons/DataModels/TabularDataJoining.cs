namespace Janus.Commons.DataModels;
public static class TabularDataJoining
{
    public static Result<TabularData> EquiJoinTabularData(TabularData foreignKeyData, TabularData primaryKeyData, string foreignKeyColumn, string primaryKeyColumn)
        => Results.AsResult(() =>
        {
            // it is expected that all the columns are named differently
            var joinedAttributeTypes = foreignKeyData.AttributeDataTypes.Union(primaryKeyData.AttributeDataTypes).ToDictionary(kv => kv.Key, kv => kv.Value);
            var tabularDataBuilder = TabularDataBuilder.InitTabularData(joinedAttributeTypes);

            foreach (var fkRow in foreignKeyData.RowData)
            {
                
                var pkRow =
                    primaryKeyData.RowData.FirstOrDefault(row => row[primaryKeyColumn].Equals(fkRow[foreignKeyColumn]))?.AttributeValues
                    ?? primaryKeyData.AttributeDataTypes.Keys.ToDictionary(k => k, k => (object?)null);

                var joinedRowValues = fkRow.AttributeValues.Union(pkRow).ToDictionary(kv => kv.Key, kv => kv.Value);

                tabularDataBuilder.AddRow(conf => conf.WithRowData(joinedRowValues));

            }

            return tabularDataBuilder.Build();
        });
}
