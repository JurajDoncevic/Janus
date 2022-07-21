using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.SchemaModels;
using Janus.Serialization.Avro.SchemaModels.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.SchemaModels;

/// <summary>
/// Avro format schema model serializer
/// </summary>
public class DataSourceSerializer : IDataSourceSerializer<byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(DataSourceDto));

    /// <summary>
    /// Deserializes a data source
    /// </summary>
    /// <param name="serialized">Serialized data source</param>
    /// <returns>Deserialized data source</returns>
    public Result<DataSource> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            var deserializedModel = AvroConvert.DeserializeHeadless<DataSourceDto>(serialized, _schema);

            var dataSource = FromDto(deserializedModel).Data!;

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
            DataSourceDto? dataSourceDto = ToDto(dataSource).Data;
            return AvroConvert.SerializeHeadless(dataSourceDto, _schema);
        });

    /// <summary>
    /// Converts a data source DTO to a data source from the schema model
    /// </summary>
    /// <param name="dataSourceDto">Data source DTO</param>
    /// <returns>Schema model data source</returns>
    internal Result<DataSource> FromDto(DataSourceDto dataSourceDto)
        => ResultExtensions.AsResult(() => 
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
                ).Build());

    /// <summary>
    /// Converts a data source schema model to its DTO
    /// </summary>
    /// <param name="dataSource">Data source schema model</param>
    /// <returns>Data source DTO</returns>
    internal Result<DataSourceDto> ToDto(DataSource dataSource)
    => ResultExtensions.AsResult(() =>
        new DataSourceDto()
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
        });
}
