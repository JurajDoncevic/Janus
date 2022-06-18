using Janus.Commons.DataModels.JsonConversion.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Commons.DataModels.JsonConversion;

public class TabularDataJsonConverter : JsonConverter<TabularData>
{
    public override TabularData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var tabularDataDto = JsonDocument.ParseValue(ref reader).Deserialize<TabularDataDto>();

        if (tabularDataDto == null)
            throw new Exception("Deserialization of TabularDataDTO failed");

        var tabularData =
            tabularDataDto.AttributeValues.Fold(
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
    }

    public override void Write(Utf8JsonWriter writer, TabularData value, JsonSerializerOptions options)
    {
        var tabularDataDto = new TabularDataDto
        {
            AttributeDataTypes = value.AttributeDataTypes.ToDictionary(kv => kv.Key, kv => kv.Value),
            AttributeValues = value.RowData.Select(rd => rd.AttributeValues.ToDictionary(kv => kv.Key, kv => kv.Value)).ToList()
        };

        var json = JsonSerializer.Serialize<TabularDataDto>(tabularDataDto);
        writer.WriteRawValue(json);
    }

}
