using Janus.Commons.SchemaModels.JsonConversion.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.SchemaModels.JsonConversion
{
    public class DataSourceJsonConverter : JsonConverter<DataSource>
    {
        public override DataSource? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonString = reader.GetString();
            var dataSourceDTO = JsonSerializer.Deserialize<DataSourceDTO>(jsonString);

            if (dataSourceDTO == null)
                throw new Exception("Deserialization of DataSourceDTO failed");

            var dataSource =
            dataSourceDTO.Schemas.Fold(SchemaModelBuilder.InitDataSource(dataSourceDTO.Name),
                (schema, dataSourceBuilder) =>
                    dataSourceBuilder.AddSchema(schema.Name, 
                        schemaBuilder => schema.Tableaus.Fold(schemaBuilder, 
                            (tableau, schemaBuilder) => schemaBuilder.AddTableau(tableau.Name, 
                                tableauBuilder => tableau.Attributes.Fold(tableauBuilder, 
                                    (attribute, tableauBuilder) => tableauBuilder.AddAttribute(attribute.Name, 
                                        attributeBuilder => attributeBuilder.WithDataType(attribute.DataType)
                                                                            .WithIsNullable(attribute.IsNullable)
                                                                            .WithIsPrimaryKey(attribute.IsPrimaryKey)
                                                                            .WithOrdinal(attribute.Ordinal)
                                    )
                                )
                            )
                        )
                    )
            ).Build();

            return dataSource;
        }

        public override void Write(Utf8JsonWriter writer, DataSource value, JsonSerializerOptions options)
        {
            DataSourceDTO dataSourceDTO = new DataSourceDTO() { Name = value.Name, Schemas = new List<SchemaDTO>() };
            foreach (var schema in value.Schemas)
            {
                // TODO
            }
        }
    }
}
