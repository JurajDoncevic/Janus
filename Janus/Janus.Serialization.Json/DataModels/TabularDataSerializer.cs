﻿using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Serialization.Json.DataModels.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Json.DataModels;

/// <summary>
/// JSON format serializer for tabular data
/// </summary>
public class TabularDataSerializer : ITabularDataSerializer<string>
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
    public Result<TabularData> Deserialize(string serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<TabularDataDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to serialize message DTO"))
            .Bind(FromDto);

    /// <summary>
    /// Serializes tabular data
    /// </summary>
    /// <param name="data">Tabular data to serialize</param>
    /// <returns>Serialized tabular data</returns>
    public Result<string> Serialize(TabularData data)
        => Results.AsResult(()
            => ToDto(data)
                .Map(dataDto => JsonSerializer.Serialize(dataDto, _serializerOptions)));

    /// <summary>
    /// Converts tabular data to its DTO
    /// </summary>
    /// <param name="tabularData">Tabular data model</param>
    /// <returns>tabular data DTO</returns>
    internal Result<TabularDataDto> ToDto(TabularData tabularData)
        => Results.AsResult(() =>
        {
            var tabularDataDto = new TabularDataDto
            {
                Name = tabularData.Name,
                AttributeDataTypes = tabularData.ColumnDataTypes.ToDictionary(kv => kv.Key, kv => kv.Value),
                AttributeValues = tabularData.RowData.Select(rd => rd.ColumnValues.ToDictionary(kv => kv.Key, kv => kv.Value)).ToList()
            };

            return tabularDataDto;
        });

    /// <summary>
    /// Converts a tabular data DTO to a data model
    /// </summary>
    /// <param name="tabularDataDto">Tabular data DTO</param>
    /// <returns>Tabular data model</returns>
    internal Result<TabularData> FromDto(TabularDataDto tabularDataDto)
        => Results.AsResult(() =>
        {
            var tabularData =
                tabularDataDto.AttributeValues.Fold(
                    TabularDataBuilder.InitTabularData(tabularDataDto.AttributeDataTypes)
                                      .WithName(tabularDataDto.Name),
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
