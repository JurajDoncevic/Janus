using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.SchemaModels;
using Janus.Commons.SchemaModels.Building;
using Janus.Serialization.Bson.SchemaModels.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        => ResultExtensions.AsResult(() => JsonSerializer.Deserialize<DataSourceDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize DataSourceDTO"))
            .Bind(FromDto);

    /// <summary>
    /// Serializes a data source
    /// </summary>
    /// <param name="dataSource">Data source to serialize</param>
    /// <returns>Serialized data source</returns>
    public Result<byte[]> Serialize(DataSource dataSource)
        => ResultExtensions.AsResult(()
            => ToDto(dataSource)
                .Map(dataSourceDto => JsonSerializer.Serialize(dataSourceDto, _serializerOptions))
                .Map(Encoding.UTF8.GetBytes));

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
                Description = dataSource.Description,
                Version = dataSource.Version,
                Schemas =
                            dataSource.Schemas.Map(schema =>
                                new SchemaDto()
                                {
                                    Name = schema.Name,
                                    Description = schema.Description,
                                    Tableaus = schema.Tableaus.Map(tableau =>
                                        new TableauDto()
                                        {
                                            Name = tableau.Name,
                                            Description = tableau.Description,
                                            Attributes = tableau.Attributes.Map(attr =>
                                                new AttributeDto()
                                                {
                                                    Name = attr.Name,
                                                    DataType = attr.DataType,
                                                    IsNullable = attr.IsNullable,
                                                    IsIdentity = attr.IsIdentity,
                                                    Ordinal = attr.Ordinal,
                                                    Description = attr.Description
                                                }).ToList(),
                                            UpdateSets = tableau.UpdateSets.Map(us => 
                                                new UpdateSetDto
                                                {
                                                    AttributeIds = us.AttributeIds
                                                }).ToHashSet()
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
                SchemaModelBuilder.InitDataSource(dataSourceDto.Name)
                    .WithDescription(dataSourceDto.Description)
                    .WithVersion(dataSourceDto.Version)
                    .AddSchemasWith(dataSourceDto.Schemas,
                        (schemaDto, scAdding) => scAdding.AddSchema(schemaDto.Name,
                            scBuilder => scBuilder.WithDescription(schemaDto.Description)
                                                  .AddTableausWith(schemaDto.Tableaus,
                                                    (tableauDto, tbAdding) => tbAdding.AddTableau(tableauDto.Name,
                                                        tbBuilder => tbBuilder.WithDescription(tableauDto.Description)
                                                                              .AddAttributesWith(tableauDto.Attributes,
                                                                                    (attributeDto, atAdding) => atAdding.AddAttribute(attributeDto.Name,
                                                                                            atBuilder => atBuilder.WithDescription(attributeDto.Description)
                                                                                                                  .WithDataType(attributeDto.DataType)
                                                                                                                  .WithIsIdentity(attributeDto.IsIdentity)
                                                                                                                  .WithIsNullable(attributeDto.IsNullable)
                                                                                                                  .WithOrdinal(attributeDto.Ordinal)
                                                                                        )
                                                                                )
                                                                              .AddUpdateSetsWith(tableauDto.UpdateSets,
                                                                                    (updateSetDto, usAdding) => usAdding.AddUpdateSet(usBuilder => usBuilder.FromEnumerable(updateSetDto.AttributeIds))
                                                                                )
                                                        )
                                                    )
                            )
                        ).Build();

            return dataSource;
        });
}
