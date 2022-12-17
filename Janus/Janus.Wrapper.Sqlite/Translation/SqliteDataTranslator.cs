using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper.Sqlite.Translation;
public sealed class SqliteDataTranslator : ILocalDataTranslator<SqliteTabularData>
{
    private readonly string _resultSchemaPrefix;

    public SqliteDataTranslator(string resultSchemaPrefix)
    {
        _resultSchemaPrefix = resultSchemaPrefix;
    }

    public Result<TabularData> TranslateToTabularData(SqliteTabularData data)
        => Results.AsResult(() =>
        {
            var attributeDataTypes = data.DataSchema.ToDictionary(kv => $"{_resultSchemaPrefix}.{kv.Key}", kv => TypeMappings.MapToDataType(kv.Value));

            var tabularData =
                data.Data.Fold(TabularDataBuilder.InitTabularData(attributeDataTypes),
                    (row, builder) => builder.AddRow(conf => conf.WithRowData(row.ToDictionary(kv => $"{_resultSchemaPrefix}.{kv.Key}", kv => kv.Value.Item2.GetType().Equals(typeof(DBNull)) ? null : kv.Value.Item2)!))
                    ).Build();

            return tabularData;
        });
}
