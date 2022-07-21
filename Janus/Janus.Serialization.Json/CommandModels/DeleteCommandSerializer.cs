using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Serialization.Json.CommandModels.DTOs;
using Janus.Serialization.Json.QueryModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Janus.Serialization.Json.CommandModels;

/// <summary>
/// JSON format serializer for the delete command
/// </summary>
public class DeleteCommandSerializer : ICommandSerializer<DeleteCommand, string>
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
    public Result<DeleteCommand> Deserialize(string serialized)
        => ResultExtensions.AsResult(() =>
        {
            var deleteDto = JsonSerializer.Deserialize<DeleteCommandDto>(serialized, _serializerOptions);

            if (deleteDto == null)
                throw new Exception("Deserialization of DeleteCommandDto failed");

            var deleteCommand = FromDto(deleteDto).Data!;

            return deleteCommand;
        });

    /// <summary>
    /// Serializes a delete command
    /// </summary>
    /// <param name="command">Delete command to serialize</param>
    /// <returns>Serialized delete command</returns>
    public Result<string> Serialize(DeleteCommand command)
        => ResultExtensions.AsResult(() =>
        {
            var deleteDto = ToDto(command).Data!;

            var json = JsonSerializer.Serialize(deleteDto, _serializerOptions);
            
            return json;
        });

    /// <summary>
    /// Converts a delete command to a DTO
    /// </summary>
    /// <param name="deleteCommand">Delete command</param>
    /// <returns>Delete commmand DTO</returns>
    internal Result<DeleteCommandDto> ToDto(DeleteCommand deleteCommand)
        => ResultExtensions.AsResult(() =>
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
        => ResultExtensions.AsResult(() =>
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
