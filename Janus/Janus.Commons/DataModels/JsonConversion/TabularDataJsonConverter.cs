using Janus.Commons.DataModels.JsonConversion.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

internal static class JsonElementExtensions
{
    public static object? ToObject(this JsonElement element, Type expectedType)
    {
            return JsonSerializer.Deserialize(element.GetRawText(), expectedType);
    }

    public static T? ToObject<T>(this JsonDocument document)
    {
        var json = document.RootElement.GetRawText();
        return JsonSerializer.Deserialize<T>(json);
    }
}
