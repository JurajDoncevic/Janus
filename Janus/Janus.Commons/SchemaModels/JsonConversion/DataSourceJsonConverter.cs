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
            var dataSourceDTO = JsonDocument.ParseValue(ref reader).Deserialize<DataSourceDTO>();

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
            DataSourceDTO dataSourceDTO = new DataSourceDTO()
            {
                Name = value.Name,
                Schemas =
                    value.Schemas.Map(schema =>
                        new SchemaDTO()
                        {
                            Name = schema.Name,
                            Tableaus = schema.Tableaus.Map(tableau =>
                                new TableauDTO()
                                {
                                    Name = tableau.Name,
                                    Attributes = tableau.Attributes.Map(attr =>
                                        new AttributeDTO()
                                        {
                                            Name = attr.Name,
                                            DataType = attr.DataType,
                                            IsNullable = attr.IsNullable,
                                            IsPrimaryKey = attr.IsPrimaryKey,
                                            Ordinal = attr.Ordinal
                                        }).ToList()
                                }).ToList()
                        }).ToList()
            };
            var json = JsonSerializer.Serialize<DataSourceDTO>(dataSourceDTO);
            writer.WriteRawValue(json);
        }
    }
}