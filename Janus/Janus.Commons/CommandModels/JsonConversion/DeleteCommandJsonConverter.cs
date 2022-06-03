using Janus.Commons.CommandModels.JsonConversion.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels.JsonConversion
{
    public class DeleteCommandJsonConverter : JsonConverter<DeleteCommand>
    {
        public override DeleteCommand? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var deleteDto = JsonDocument.ParseValue(ref reader).Deserialize<DeleteCommandDto>();

            if (deleteDto == null)
                throw new Exception("Deserialization of DeleteCommandDto failed");

            var deleteCommand = 
            DeleteCommandOpenBuilder.InitOpenDelete(deleteDto.OnTableauId)
                .WithSelection(conf => deleteDto.Selection != null
                                       ? conf.WithExpression(deleteDto.Selection.SelectionExpression)
                                       : conf)
                .Build();

            return deleteCommand;
        }

        public override void Write(Utf8JsonWriter writer, DeleteCommand value, JsonSerializerOptions options)
        {
            var deleteDto = new DeleteCommandDto(
                value.OnTableauId, 
                value.Selection 
                    ? new CommandSelectionDto() { SelectionExpression = value.Selection.Value.Expression } 
                    : null
                );

            var json = JsonSerializer.Serialize(deleteDto);
            writer.WriteRawValue(json);
        }
    }
}
