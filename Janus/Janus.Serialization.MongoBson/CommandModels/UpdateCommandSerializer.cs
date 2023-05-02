using Janus.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Serialization.MongoBson.CommandModels.DTOs;
using Janus.Serialization.MongoBson.QueryModels;
using System.Text;

namespace Janus.Serialization.MongoBson.CommandModels;

/// <summary>
/// MongoBson format serializer for the update command
/// </summary>
public sealed class UpdateCommandSerializer : ICommandSerializer<UpdateCommand, byte[]>
{
    private readonly SelectionExpressionConverter _selectionExpressionConverter = new SelectionExpressionConverter();

    /// <summary>
    /// Deserializes an update command
    /// </summary>
    /// <param name="serialized">Serialized update command</param>
    /// <returns>Deserialized update command</returns>
    public Result<UpdateCommand> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromBson<UpdateCommandDto>(serialized))
            .Bind(FromDto);

    /// <summary>
    /// Serializes an update command
    /// </summary>
    /// <param name="command">Update command to serialize</param>
    /// <returns>Serialized update command</returns>
    public Result<byte[]> Serialize(UpdateCommand command)
        => Results.AsResult(()
            => ToDto(command)
                .Map(updateCommandDto => Utils.ToBson(updateCommandDto))
        );

    /// <summary>
    /// Converts an update command to its DTO
    /// </summary>
    /// <param name="command">Update command model</param>
    /// <returns>Update command DTO</returns>
    internal Result<UpdateCommandDto> ToDto(UpdateCommand command)
        => Results.AsResult(() =>
        {
            var updateCommandDto = new UpdateCommandDto(
                command.OnTableauId.ToString(),
                command.Mutation.ValueUpdates.ToDictionary(kv => kv.Key, kv => ConvertToBytes(kv.Value, kv.Value?.GetType() ?? typeof(object))),
                command.Mutation.ValueUpdates.ToDictionary(kv => kv.Key, kv => kv.Value?.GetType().ToString() ?? typeof(byte[]).ToString()),
                command.Selection.IsSome
                            ? new CommandSelectionDto() { SelectionExpression = _selectionExpressionConverter.ToStringExpression(command.Selection.Value.Expression) }
                            : null,
                command.Name
                );

            return updateCommandDto;
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
                kv => kv.Value?.Length == 0 ? null : ConvertFromBytes(kv.Value!, TypeNameToType(updateCommandDto.MutationTypes[kv.Key]))
                );

            var updateCommand =
            UpdateCommandOpenBuilder.InitOpenUpdate(updateCommandDto.OnTableauId)
                .WithName(updateCommandDto.Name)
                .WithMutation(conf => conf.WithValues(retypedMutationDict))
                .WithSelection(conf => updateCommandDto.Selection != null
                                        ? conf.WithExpression(_selectionExpressionConverter.FromStringExpression(updateCommandDto.Selection.SelectionExpression)!)
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

    /// <summary>
    /// Converts primitive data to a byte array
    /// </summary>
    /// <param name="value"></param>
    /// <param name="originalType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private byte[]? ConvertToBytes(object? value, Type originalType)
        => value == null ? Array.Empty<byte>() : originalType switch
        {
            Type t when t == typeof(int) => BitConverter.GetBytes((int)value),
            Type t when t == typeof(long) => BitConverter.GetBytes((long)value),
            Type t when t == typeof(double) => BitConverter.GetBytes((double)value),
            Type t when t == typeof(bool) => BitConverter.GetBytes((bool)value),
            Type t when t == typeof(DateTime) => BitConverter.GetBytes(DateTime.Now.Ticks),
            Type t when t == typeof(string) => Encoding.UTF8.GetBytes(value.ToString()),
            Type t when t == typeof(byte[]) => (byte[])value,
            _ => throw new ArgumentException($"No mapping for Type {originalType.FullName}")
        };

    /// <summary>
    /// Converts a byte array to primitive data
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="expectedType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private object? ConvertFromBytes(byte[] bytes, Type expectedType)
        => bytes.Length == 0 ? null : expectedType switch
        {
            Type t when t == typeof(int) => BitConverter.ToInt32(bytes),
            Type t when t == typeof(long) => BitConverter.ToInt64(bytes),
            Type t when t == typeof(double) => BitConverter.ToDouble(bytes),
            Type t when t == typeof(bool) => BitConverter.ToBoolean(bytes),
            Type t when t == typeof(DateTime) => new DateTime(BitConverter.ToInt64(bytes)),
            Type t when t == typeof(string) => Encoding.UTF8.GetString(bytes),
            Type t when t == typeof(byte[]) => (byte[])bytes,
            _ => throw new ArgumentException($"No mapping for Type {expectedType.FullName}")
        };
}
