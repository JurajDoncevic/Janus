using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Mask.LiteDB.MaskedSchemaModel;
using Janus.Mask.Translation;

namespace Janus.Mask.LiteDB.Translation;
public sealed class LiteDbSchemaTranslator : IMaskSchemaTranslator<Database>
{
    public Result<Database> Translate(DataSource source)
        => Results.AsResult(() =>
        {
            var builder = LiteDbSchemaModelBuilder.Init(source.Name);

            foreach (var schema in source.Schemas)
            {
                foreach (var tableau in schema.Tableaus)
                {
                    builder.AddCollection($"{schema.Name}_{tableau.Name}", collectionConf =>
                        collectionConf.WithName($"{schema.Name}_{tableau.Name}")
                                      .AddDocument(documentConf =>
                                      {
                                          documentConf = documentConf.WithIndex(0);

                                          foreach (var attribute in tableau.Attributes)
                                          {
                                              documentConf =
                                                documentConf.WithPrimitiveField(attribute.Name, fieldConf =>
                                                    {
                                                        fieldConf =
                                                            SetFieldType(fieldConf, attribute.DataType)
                                                            .WithName(attribute.Name);
                                                        fieldConf =
                                                            attribute.IsIdentity ? fieldConf.AsIdentity() : fieldConf;

                                                        return fieldConf;
                                                    });
                                          }

                                          return documentConf;
                                      }));
                }
            }

            var databaseSchema = builder.Build();

            return databaseSchema;
        });

    private PrimitiveFieldBuilder SetFieldType(PrimitiveFieldBuilder builder, DataTypes dataType)
        => dataType switch
        {
            DataTypes.INT => builder.AsInt32(),
            DataTypes.LONGINT => builder.AsInt64(),
            DataTypes.BOOLEAN => builder.AsString(),
            DataTypes.DECIMAL => builder.AsDouble(),
            DataTypes.DATETIME => builder.AsString(),
            DataTypes.BINARY => builder.AsString(),
            _ => throw new NotSupportedException("Unknown data type provided")
        };
}
