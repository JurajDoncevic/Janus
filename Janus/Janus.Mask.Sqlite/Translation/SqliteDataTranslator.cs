using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Mask.Sqlite.MaskedDataModel;
using Janus.Mask.Sqlite.MaskedSchemaModel;
using Janus.Wrapper.Translation;

namespace Janus.Mask.Sqlite.Translation;
public sealed class SqliteDataTranslator : IMaskDataTranslator<SqliteTabularData, SqliteDataRow>
{
    public Result<TabularData> Translate(SqliteTabularData data)
        => Results.AsResult(() =>
        {
            var attributeDataTypes = data.DataSchema.ToDictionary(kv => $"{kv.Key}", kv => MapToDataType(kv.Value));

            var tabularData =
                data.Data.Fold(TabularDataBuilder.InitTabularData(attributeDataTypes),
                    (row, builder) => builder.AddRow(conf => conf.WithRowData(row.DataRow.ToDictionary(kv => $"{kv.Key}", kv => kv.Value)!))
                    ).Build();

            return tabularData;
        });

    public Result<SqliteTabularData> Translate(TabularData destination)
        => Results.AsResult(() =>
        {
            return new SqliteTabularData(
                destination.RowData.Map(row => new SqliteDataRow(new Dictionary<string, object?>(row.ColumnValues))).ToList(),
                destination.ColumnDataTypes.ToDictionary(kv => kv.Key, kv => MapToTypeAffinity(kv.Value))
                );
        });


    private TypeAffinities MapToTypeAffinity(DataTypes dataType)
        => dataType switch
        {
            DataTypes.LONGINT or DataTypes.INT => TypeAffinities.INTEGER,
            DataTypes.DECIMAL => TypeAffinities.REAL,
            DataTypes.DATETIME => TypeAffinities.DATETIME,
            DataTypes.BOOLEAN => TypeAffinities.BOOLEAN,
            DataTypes.STRING => TypeAffinities.TEXT,
            DataTypes.BINARY => TypeAffinities.BLOB,
            _ => TypeAffinities.TEXT
        };

    private DataTypes MapToDataType(TypeAffinities dataType)
        => dataType switch
        {
            TypeAffinities.INTEGER => DataTypes.INT,
            TypeAffinities.REAL => DataTypes.DECIMAL,
            TypeAffinities.DATETIME => DataTypes.DATETIME,
            TypeAffinities.BOOLEAN => DataTypes.BOOLEAN,
            TypeAffinities.TEXT => DataTypes.STRING,
            TypeAffinities.BLOB => DataTypes.BINARY,
            _ => DataTypes.STRING
        };
}
