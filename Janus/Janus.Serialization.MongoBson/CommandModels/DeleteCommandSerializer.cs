using FunctionalExtensions.Base.Results;
using Janus.Commons.CommandModels;
using Janus.Serialization.MongoBson.CommandModels.DTOs;
using Janus.Serialization.MongoBson.QueryModels;

namespace Janus.Serialization.MongoBson.CommandModels;

/// <summary>
/// MongoBson format serializer for the delete command
/// </summary>
public class DeleteCommandSerializer : ICommandSerializer<DeleteCommand, byte[]>
{
    private readonly SelectionExpressionConverter _selectionExpressionConverter = new SelectionExpressionConverter();

    /// <summary>
    /// Deserializes a delete command
    /// </summary>
    /// <param name="serialized">Serialized delete command</param>
    /// <returns>Deserialized delete command</returns>
    public Result<DeleteCommand> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() => Utils.FromBson<DeleteCommandDto>(serialized))
            .Bind(FromDto);

    /// <summary>
    /// Serializes a delete command
    /// </summary>
    /// <param name="command">Delete command to serialize</param>
    /// <returns>Serialized delete command</returns>
    public Result<byte[]> Serialize(DeleteCommand command)
        => ResultExtensions.AsResult(()
            => ToDto(command)
                .Map(deleteCommandDto => Utils.ToBson(deleteCommandDto))
        );

    /// <summary>
    /// Converts a delete command to a DTO
    /// </summary>
    /// <param name="deleteCommand">Delete command</param>
    /// <returns>Delete commmand DTO</returns>
    internal Result<DeleteCommandDto> ToDto(DeleteCommand deleteCommand)
        => ResultExtensions.AsResult(() =>
        {
            var deleteCommandDto = new DeleteCommandDto(
                deleteCommand.OnTableauId,
                deleteCommand.Selection
                    ? new CommandSelectionDto() { SelectionExpression = _selectionExpressionConverter.ToStringExpression(deleteCommand.Selection.Value.Expression) }
                    : null
                );

            return deleteCommandDto;
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
                .WithSelection(conf => deleteCommandDto.Selection == null
                                        ? conf
                                        : conf.WithExpression(_selectionExpressionConverter.FromStringExpression(deleteCommandDto.Selection.SelectionExpression)!))
                .Build();

            return deleteCommand;
        });
}
