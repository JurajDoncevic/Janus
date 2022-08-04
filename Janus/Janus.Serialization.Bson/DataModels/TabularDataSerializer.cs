﻿using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.DataModels;
using Janus.Serialization.Bson.DataModels.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.DataModels;

/// <summary>
/// BSON format serializer for tabular data
/// </summary>
public class TabularDataSerializer : ITabularDataSerializer<byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public TabularDataSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes tabular data
    /// </summary>
    /// <param name="serialized">Serialized tabular data</param>
    /// <returns>Deserialized tabular data</returns>
    public Result<TabularData> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() => JsonSerializer.Deserialize<TabularDataDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize TabularDataDTO"))
            .Bind(FromDto);

    /// <summary>
    /// Serializes tabular data
    /// </summary>
    /// <param name="data">Tabular data to serialize</param>
    /// <returns>Serialized tabular data</returns>
    public Result<byte[]> Serialize(TabularData data)
        => ResultExtensions.AsResult(()
            => ToDto(data)
                .Map(dataDto => JsonSerializer.Serialize(dataDto, _serializerOptions))
                .Map(Encoding.UTF8.GetBytes));

    /// <summary>
    /// Converts tabular data to its DTO
    /// </summary>
    /// <param name="tabularData">Tabular data model</param>
    /// <returns>tabular data DTO</returns>
    internal Result<TabularDataDto> ToDto(TabularData? tabularData)
        => ResultExtensions.AsResult(() =>
        {
            var tabularDataDto = new TabularDataDto
            {
                AttributeDataTypes = tabularData.AttributeDataTypes.ToDictionary(kv => kv.Key, kv => kv.Value),
                AttributeValues = tabularData.RowData.Select(rd => rd.AttributeValues.ToDictionary(kv => kv.Key, kv => kv.Value)).ToList()
            };

            return tabularDataDto;
        });

    /// <summary>
    /// Converts a tabular data DTO to a data model
    /// </summary>
    /// <param name="tabularDataDto">Tabular data DTO</param>
    /// <returns>Tabular data model</returns>
    internal Result<TabularData?> FromDto(TabularDataDto? tabularDataDto)
        => ResultExtensions.AsResult(() =>
        {
            var tabularData =
                tabularDataDto?.AttributeValues.Fold(
                    TabularDataBuilder.InitTabularData(tabularDataDto.AttributeDataTypes),
                    (attrVals, builder) => builder.AddRow(
                        conf => conf.WithRowData(attrVals.ToDictionary(
                            av => av.Key,
                            av => av.Value != null
                                  ? JsonSerializer.Deserialize(((JsonElement)av.Value), TypeMappings.MapToType(tabularDataDto.AttributeDataTypes[av.Key]))
                                  : null
                            )))
                    ).Build();

            return tabularData;
        });
}