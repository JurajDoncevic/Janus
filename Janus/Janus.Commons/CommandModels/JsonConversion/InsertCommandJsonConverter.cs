using Janus.Commons.CommandModels.JsonConversion.DTOs;
using Janus.Commons.DataModels;
using Janus.Commons.DataModels.JsonConversion.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels.JsonConversion;

public class InsertCommandJsonConverter : JsonConverter<InsertCommand>
{
    public override InsertCommand? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var insertDto = JsonDocument.ParseValue(ref reader).Deserialize<InsertCommandDto>();
        if (insertDto == null)
            throw new Exception("Deserialization of InsertCommandDto failed");

        var tabularData = insertDto.Instantiation.AttributeValues.Fold(
                TabularDataBuilder.InitTabularData(insertDto.Instantiation.AttributeDataTypes),
                (rowData, builder) => builder.AddRow(
                    conf => conf.WithRowData(rowData.ToDictionary(
                        av => av.Key,
                        av => av.Value != null
                              ? JsonSerializer.Deserialize(((JsonElement)av.Value), TypeMappings.MapToType(insertDto.Instantiation.AttributeDataTypes[av.Key]))
                              : null
                        ))
                )
            ).Build();

        ;
        var insertCommand = InsertCommandOpenBuilder.InitOpenInsert(insertDto.OnTableauId)
                                .WithInstantiation(conf => conf.WithValues(tabularData))
                                .Build();

        return insertCommand;
    }

    public override void Write(Utf8JsonWriter writer, InsertCommand value, JsonSerializerOptions options)
    {
        var command = value;
        var tabularData = value.Instantiation.TabularData;
        var insertDto = new InsertCommandDto()
        {
            OnTableauId = value.OnTableauId,
            Instantiation = new TabularDataDto
            {
                AttributeDataTypes = tabularData.AttributeDataTypes.ToDictionary(kv => kv.Key, kv => kv.Value),
                AttributeValues = tabularData.RowData.Select(rd => rd.AttributeValues.ToDictionary(kv => kv.Key, kv => kv.Value)).ToList()
            }
        };

        var json = JsonSerializer.Serialize(insertDto);
        writer.WriteRawValue(json);
    }
}
