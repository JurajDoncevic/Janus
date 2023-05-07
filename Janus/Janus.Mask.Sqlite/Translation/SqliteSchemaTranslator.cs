using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Mask.Sqlite.MaskedSchemaModel;
using Janus.Mask.Translation;

namespace Janus.Mask.Sqlite.Translation;
public sealed class SqliteSchemaTranslator : IMaskSchemaTranslator<Database>
{
    public Result<Database> Translate(DataSource source)
        => Results.AsResult(() =>
        {
            var databaseBuilder =
            source.Schemas.SelectMany(schema => schema.Tableaus)
                .Fold(SqliteSchemaModelBuilder.Init(source.Name),
                (tableau, databaseBuilder) =>
                {
                    var dbBuilder = databaseBuilder.AddTable($"{tableau.Schema.Name}_{tableau.Name}", tableBuilder =>
                    {
                        return tableau.Attributes.Fold(tableBuilder, (attr, tblBuilder) =>
                        {
                            return tblBuilder.AddColumn(attr.Name, colBuilder =>
                            {
                                var bldr = colBuilder
                                    .WithName(attr.Name)
                                    .WithTypeAffinity(MapToTypeAffinity(attr.DataType));

                                bldr = attr.IsIdentity ? bldr.AsPrimaryKey() : bldr;

                                return bldr;
                            });
                        });
                    });

                    return dbBuilder;
                });

            return databaseBuilder.Build();
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
}
