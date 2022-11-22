using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Commons.SchemaModels.Building;
using Janus.Serialization.Protobufs.SchemaModels.DTOs;

namespace Janus.Serialization.Protobufs.SchemaModels;

/// <summary>
/// Protobufs format schema model serializer
/// </summary>
public class DataSourceSerializer : IDataSourceSerializer<byte[]>
{
    /// <summary>
    /// Deserializes a data source
    /// </summary>
    /// <param name="serialized">Serialized data source</param>
    /// <returns>Deserialized data source</returns>
    public Result<DataSource> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromProtobufs<DataSourceDto>(serialized))
            .Bind(FromDto);

    /// <summary>
    /// Serializes a data source
    /// </summary>
    /// <param name="dataSource">Data source to serialize</param>
    /// <returns>Serialized data source</returns>
    public Result<byte[]> Serialize(DataSource dataSource)
        => Results.AsResult(()
            => ToDto(dataSource)
                .Map(dataSourceDto => Utils.ToProtobufs(dataSourceDto))
        );

    /// <summary>
    /// Converts a data source DTO to a data source from the schema model
    /// </summary>
    /// <param name="dataSourceDto">Data source DTO</param>
    /// <returns>Schema model data source</returns>
    internal Result<DataSource> FromDto(DataSourceDto dataSourceDto)
    => Results.AsResult(() =>
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
                                                                                (updateSetDto, usAdding) => usAdding.AddUpdateSet(usBuilder => usBuilder.WithAttributesNamed(updateSetDto.AttributeIds))
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
    => Results.AsResult(() =>
        new DataSourceDto()
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
                                            AttributeIds = us.AttributeNames.ToHashSet()
                                        }).ToHashSet()
                                }).ToList()
                        }).ToList()
        });
}
