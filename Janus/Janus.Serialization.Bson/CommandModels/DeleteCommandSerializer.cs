using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Serialization.Bson.CommandModels.DTOs;
using Janus.Serialization.Bson.QueryModels;
using System.Text;
using System.Text.Json;

namespace Janus.Serialization.Bson.CommandModels;

/// <summary>
/// BSON format serializer for the delete command
/// </summary>
public sealed class DeleteCommandSerializer : ICommandSerializer<DeleteCommand, byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public DeleteCommandSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new SelectionExpressionJsonConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a delete command
    /// </summary>
    /// <param name="serialized">Serialized delete command</param>
    /// <returns>Deserialized delete command</returns>
    public Result<DeleteCommand> Deserialize(byte[] serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<DeleteCommandDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize DeleteCommandDTO"))
            .Bind(FromDto);

    /// <summary>
    /// Serializes a delete command
    /// </summary>
    /// <param name="command">Delete command to serialize</param>
    /// <returns>Serialized delete command</returns>
    public Result<byte[]> Serialize(DeleteCommand command)
        => Results.AsResult(()
            => ToDto(command)
                .Map(deleteDto => JsonSerializer.Serialize(deleteDto, _serializerOptions))
                .Map(Encoding.UTF8.GetBytes));

    /// <summary>
    /// Converts a delete command to a DTO
    /// </summary>
    /// <param name="deleteCommand">Delete command</param>
    /// <returns>Delete commmand DTO</returns>
    internal Result<DeleteCommandDto> ToDto(DeleteCommand deleteCommand)
        => Results.AsResult(() =>
        {
            var deleteDto = new DeleteCommandDto(
                deleteCommand.OnTableauId,
                deleteCommand.Selection
                    ? new CommandSelectionDto() { SelectionExpression = deleteCommand.Selection.Value.Expression }
                    : null
                );
            return deleteDto;
        });

    /// <summary>
    /// Converts a delete command DTO to the command model
    /// </summary>
    /// <param name="deleteCommandDto">Delete command DTO</param>
    /// <returns>Delete command model</returns>
    internal Result<DeleteCommand> FromDto(DeleteCommandDto deleteCommandDto)
        => Results.AsResult(() =>
        {

            var deleteCommand =
                DeleteCommandOpenBuilder.InitOpenDelete(deleteCommandDto.OnTableauId)
                    .WithSelection(conf => deleteCommandDto.Selection != null
                                           ? conf.WithExpression(deleteCommandDto.Selection.SelectionExpression)
                                           : conf)
                    .Build();
            return deleteCommand;
        });
}
