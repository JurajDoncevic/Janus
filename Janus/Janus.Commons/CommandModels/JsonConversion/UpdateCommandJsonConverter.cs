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
            kv => kv.Value != null ? JsonSerializer.Deserialize(((JsonElement)kv.Value), TypeNameToType(updateDto.MutationTypes[kv.Key])) : null
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
        var updateDto = new UpdateCommandDto(
            value.OnTableauId,
            value.Mutation.ValueUpdates.ToDictionary(kv => kv.Key, kv => kv.Value),
            value.Selection.IsSome
                        ? new CommandSelectionDto() { SelectionExpression = value.Selection.Value.Expression }
                        : null
            );
        var json = JsonSerializer.Serialize(updateDto);
        writer.WriteRawValue(json);
    }

    private Type TypeNameToType(string typeName) =>
        typeName switch
        {
            string tn when tn.Equals(typeof(int).FullName) => typeof(int),
            string tn when tn.Equals(typeof(double).FullName) => typeof(double),
            string tn when tn.Equals(typeof(string).FullName) => typeof(string),
            string tn when tn.Equals(typeof(DateTime).FullName) => typeof(DateTime),
            string tn when tn.Equals(typeof(byte[]).FullName) => typeof(byte[]),
            string tn when tn.Equals(typeof(bool).FullName) => typeof(bool),
            _ => throw new Exception($"Unknown type name {typeName}")
        };
}
