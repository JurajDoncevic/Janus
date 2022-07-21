using FunctionalExtensions.Base.Results;
using System.Text.Json;
using System.Text.Json.Serialization;
using Janus.Commons.SchemaModels;
using Janus.Serialization.Bson.SchemaModels.DTOs;
using FunctionalExtensions.Base;
using System.Text;
using System.Text.Json.Nodes;

namespace Janus.Serialization.Bson.SchemaModels;

/// <summary>
/// BSON format schema model serializer
/// </summary>
public class DataSourceSerializer : IDataSourceSerializer<byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public DataSourceSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a data source
    /// </summary>
    /// <param name="serialized">Serialized data source</param>
    /// <returns>Deserialized data source</returns>
    public Result<DataSource> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            var dataSourceDto = JsonSerializer.Deserialize<DataSourceDto>(serialized, _serializerOptions);

            if (dataSourceDto == null)
                throw new Exception("Deserialization of DataSourceDTO failed");
            if (dataSourceDto == null)
                throw new Exception("Deserialization of DataSourceDTO failed");

            var dataSource = FromDto(dataSourceDto).Data!;

            return dataSource;
        });

    /// <summary>
    /// Serializes a data source
    /// </summary>
    /// <param name="dataSource">Data source to serialize</param>
    /// <returns>Serialized data source</returns>
    public Result<byte[]> Serialize(DataSource dataSource)
        => ResultExtensions.AsResult(() =>
        {
            DataSourceDto dataSourceDto = ToDto(dataSource).Data!;
            var json = JsonSerializer.Serialize<DataSourceDto>(dataSourceDto, _serializerOptions);
            var bson = Encoding.UTF8.GetBytes(json);

            return bson;
        });

    /// <summary>
    /// Converts a data source schema model to its DTO
    /// </summary>
    /// <param name="dataSource">Data source schema model</param>
    /// <returns>Data source DTO</returns>
    internal Result<DataSourceDto> ToDto(DataSource dataSource)
        => ResultExtensions.AsResult(() =>
        {
            DataSourceDto dataSourceDto = new DataSourceDto()
            {
                Name = dataSource.Name,
                Schemas =
                            dataSource.Schemas.Map(schema =>
                                new SchemaDto()
                                {
                                    Name = schema.Name,
                                    Tableaus = schema.Tableaus.Map(tableau =>
                                        new TableauDto()
                                        {
                                            Name = tableau.Name,
                                            Attributes = tableau.Attributes.Map(attr =>
                                                new AttributeDto()
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

            return dataSourceDto;
        });

    /// <summary>
    /// Converts a data source DTO to a data source from the schema model
    /// </summary>
    /// <param name="dataSourceDto">Data source DTO</param>
    /// <returns>Schema model data source</returns>
    internal Result<DataSource> FromDto(DataSourceDto dataSourceDto)
        => ResultExtensions.AsResult(() =>
        {
            var dataSource =
                dataSourceDto.Schemas.Fold(SchemaModelBuilder.InitDataSource(dataSourceDto.Name),
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
        });
}
