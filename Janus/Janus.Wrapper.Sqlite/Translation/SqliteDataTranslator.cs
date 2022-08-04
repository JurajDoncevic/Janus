using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.DataModels;
using Janus.Wrapper.Sqlite.LocalDataModel;
using Janus.Wrapper.Translation;

namespace Janus.Wrapper.Sqlite.Translation;
public class SqliteDataTranslator : ILocalDataTranslator<SqliteTabularData>
{
    public Result<TabularData> TranslateToTabularData(SqliteTabularData data)
        => ResultExtensions.AsResult(() =>
        {
            var attributeDataTypes = data.DataSchema.ToDictionary(kv => kv.Key, kv => TypeMappings.MapToDataType(kv.Value));

            var tabularData =
                data.Data.Fold(TabularDataBuilder.InitTabularData(attributeDataTypes),
                    (row, builder) => builder.AddRow(conf => conf.WithRowData(row.ToDictionary(kv => kv.Key, kv => kv.Value.Item2)!))
                    ).Build();

            return tabularData;
        });
}
