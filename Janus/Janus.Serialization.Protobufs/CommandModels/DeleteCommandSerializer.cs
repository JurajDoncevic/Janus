using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Serialization.Protobufs.CommandModels.DTOs;
using Janus.Serialization.Protobufs.QueryModels;

namespace Janus.Serialization.Protobufs.CommandModels;

/// <summary>
/// Protobufs format serializer for the delete command
/// </summary>
public sealed class DeleteCommandSerializer : ICommandSerializer<DeleteCommand, byte[]>
{
    private readonly SelectionExpressionConverter _selectionExpressionConverter = new SelectionExpressionConverter();

    /// <summary>
    /// Deserializes a delete command
    /// </summary>
    /// <param name="serialized">Serialized delete command</param>
    /// <returns>Deserialized delete command</returns>
    public Result<DeleteCommand> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromProtobufs<DeleteCommandDto>(serialized))
            .Bind(FromDto);

    /// <summary>
    /// Serializes a delete command
    /// </summary>
    /// <param name="command">Delete command to serialize</param>
    /// <returns>Serialized delete command</returns>
    public Result<byte[]> Serialize(DeleteCommand command)
        => Results.AsResult(()
            => ToDto(command)
                .Map(deleteCommandDto => Utils.ToProtobufs(deleteCommandDto))
        );

    /// <summary>
    /// Converts a delete command to a DTO
    /// </summary>
    /// <param name="deleteCommand">Delete command</param>
    /// <returns>Delete commmand DTO</returns>
    internal Result<DeleteCommandDto> ToDto(DeleteCommand deleteCommand)
        => Results.AsResult(() =>
        {
            var deleteCommandDto = new DeleteCommandDto(
                deleteCommand.OnTableauId.ToString(),
                deleteCommand.Selection
                    ? new CommandSelectionDto() { SelectionExpression = _selectionExpressionConverter.ToStringExpression(deleteCommand.Selection.Value.Expression) }
                    : null,
                deleteCommand.Name
                );

            return deleteCommandDto;
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
                .WithName(deleteCommandDto.Name)
                .WithSelection(conf => deleteCommandDto.Selection == null
                                        ? conf
                                        : conf.WithExpression(_selectionExpressionConverter.FromStringExpression(deleteCommandDto.Selection.SelectionExpression)!))
                .Build();

            return deleteCommand;
        });
}
