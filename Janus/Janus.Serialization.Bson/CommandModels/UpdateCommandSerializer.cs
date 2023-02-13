using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Serialization.Bson.CommandModels.DTOs;
using Janus.Serialization.Bson.QueryModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.CommandModels;

/// <summary>
/// BSON format serializer for the update command
/// </summary>
public sealed class UpdateCommandSerializer : ICommandSerializer<UpdateCommand, byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public UpdateCommandSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new SelectionExpressionJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes an update command
    /// </summary>
    /// <param name="serialized">Serialized update command</param>
    /// <returns>Deserialized update command</returns>
    public Result<UpdateCommand> Deserialize(byte[] serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<UpdateCommandDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize UpdateCommandDTO"))
            .Bind(FromDto);

    /// <summary>
    /// Serializes an update command
    /// </summary>
    /// <param name="command">Update command to serialize</param>
    /// <returns>Serialized update command</returns>
    public Result<byte[]> Serialize(UpdateCommand command)
        => Results.AsResult(()
            => ToDto(command)
                .Map(updateDto => JsonSerializer.Serialize(updateDto, _serializerOptions))
                .Map(Encoding.UTF8.GetBytes));

    /// <summary>
    /// Converts an update command to its DTO
    /// </summary>
    /// <param name="command">Update command model</param>
    /// <returns>Update command DTO</returns>
    internal Result<UpdateCommandDto> ToDto(UpdateCommand updateCommand)
        => Results.AsResult(() =>
        {
            var updateDto = new UpdateCommandDto(
                updateCommand.OnTableauId.ToString(),
                updateCommand.Mutation.ValueUpdates.ToDictionary(kv => kv.Key, kv => kv.Value),
                updateCommand.Selection.IsSome
                            ? new CommandSelectionDto() { SelectionExpression = updateCommand.Selection.Value.Expression }
                            : null,
                updateCommand.Name
                );

            return updateDto;
        });

    /// <summary>
    /// Converts an update command DTO to the command model
    /// </summary>
    /// <param name="updateCommandDto">Update command DTO</param>
    /// <returns>Update command model</returns>
    internal Result<UpdateCommand> FromDto(UpdateCommandDto updateCommandDto)
        => Results.AsResult(() =>
        {
            var retypedMutationDict = updateCommandDto.Mutation.ToDictionary(
                kv => kv.Key,
                kv => kv.Value != null ? JsonSerializer.Deserialize(((JsonElement)kv.Value), TypeNameToType(updateCommandDto.MutationTypes[kv.Key])) : null
                );

            var updateCommand =
            UpdateCommandOpenBuilder.InitOpenUpdate(updateCommandDto.OnTableauId)
                .WithName(updateCommandDto.Name)
                .WithMutation(conf => conf.WithValues(retypedMutationDict))
                .WithSelection(conf => updateCommandDto.Selection != null
                                        ? conf.WithExpression(updateCommandDto.Selection.SelectionExpression)
                                        : conf)
                .Build();

            return updateCommand;
        });

    /// <summary>
    /// Gets a concrete type for a type name
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Type TypeNameToType(string typeName) =>
        typeName switch
        {
            string tn when tn.Equals(typeof(int).FullName) => typeof(int),
            string tn when tn.Equals(typeof(long).FullName) => typeof(long),
            string tn when tn.Equals(typeof(double).FullName) => typeof(double),
            string tn when tn.Equals(typeof(string).FullName) => typeof(string),
            string tn when tn.Equals(typeof(DateTime).FullName) => typeof(DateTime),
            string tn when tn.Equals(typeof(byte[]).FullName) => typeof(byte[]),
            string tn when tn.Equals(typeof(bool).FullName) => typeof(bool),
            _ => throw new Exception($"Unknown type name {typeName}")
        };
}
