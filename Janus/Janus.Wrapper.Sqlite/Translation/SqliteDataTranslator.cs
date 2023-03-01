using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper.Sqlite.Translation;
public sealed class SqliteDataTranslator : IWrapperDataTranslator<SqliteTabularData>
{
    private readonly string _resultSchemaPrefix;

    public SqliteDataTranslator(string resultSchemaPrefix)
    {
        _resultSchemaPrefix = resultSchemaPrefix;
    }

    public Result<TabularData> Translate(SqliteTabularData data)
        => Results.AsResult(() =>
        {
            var attributeDataTypes = data.DataSchema.ToDictionary(kv => $"{_resultSchemaPrefix}.{kv.Key}", kv => TypeMappings.MapToDataType(kv.Value));

            var tabularData =
                data.DataRows.Fold(TabularDataBuilder.InitTabularData(attributeDataTypes),
                    (row, builder) => builder.AddRow(conf => conf.WithRowData(row.ToDictionary(kv => $"{_resultSchemaPrefix}.{kv.Key}", kv => kv.Value)!))
                    ).Build();

            return tabularData;
        });

    public Result<SqliteTabularData> Translate(TabularData destination)
        => Results.AsResult(() =>
        {
            return new SqliteTabularData(
                destination.ColumnDataTypes.ToDictionary(kv => kv.Key, kv => TypeMappings.MapToType(kv.Value)),
                destination.RowData.Map(row => new Dictionary<string, object?>(row.ColumnValues)).ToList()
                );
        });
}
