using Janus.Commons.CommandModels.JsonConversion.DTOs;
using Janus.Commons.SelectionExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.CommandModels.JsonConversion;

public class UpdateCommandJsonConverter : JsonConverter<UpdateCommand>
{
    public override UpdateCommand? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var updateDto = JsonDocument.ParseValue(ref reader).Deserialize<UpdateCommandDto>();

        if(updateDto == null)
            throw new Exception("Deserialization of UpdateCommandDto failed");

        var retypedMutationDict = updateDto.Mutation.ToDictionary(
            kv => kv.Key,
            kv => JsonSerializer.Deserialize(((JsonElement)kv.Value.Item1), kv.Value.Item2)
            );

        var tempBuild =
        UpdateCommandOpenBuilder.InitOpenUpdate(updateDto.OnTableauId)
            .WithMutation(conf => conf.WithValues(retypedMutationDict))
            .WithSelection(conf => updateDto.Selection != null
                                    ? conf.WithExpression(updateDto.Selection.SelectionExpression)
                                    : conf);


        return tempBuild.Build();

    }

    public override void Write(Utf8JsonWriter writer, UpdateCommand value, JsonSerializerOptions options)
    {
        var updateDto = new UpdateCommandDto
        {
            OnTableauId = value.OnTableauId,
            Mutation = value.Mutation.ValueUpdates.ToDictionary(kv => kv.Key, kv => (kv.Value, kv.Value?.GetType() ?? typeof(object))),
            Selection = value.Selection.IsSome
                        ? new CommandSelectionDto() { SelectionExpression = value.Selection.Value.Expression }
                        : new CommandSelectionDto()
        };

        writer.WriteRawValue(JsonSerializer.Serialize(updateDto));
    }
}
