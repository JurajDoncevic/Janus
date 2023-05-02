using Janus.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.Messages;
using Janus.Serialization.Protobufs.CommandModels;
using Janus.Serialization.Protobufs.Messages.DTOs;

namespace Janus.Serialization.Protobufs.Messages;

/// <summary>
/// Protobufs format COMMAND_REQ message serializer
/// </summary>
public sealed class CommandReqMessageSerializer : IMessageSerializer<CommandReqMessage, byte[]>
{
    private readonly DeleteCommandSerializer _deleteCommandSerializer = new();
    private readonly InsertCommandSerializer _insertCommandSerializer = new();
    private readonly UpdateCommandSerializer _updateCommandSerializer = new();

    /// <summary>
    /// Deserializes a COMMAND_REQ message
    /// </summary>
    /// <param name="serialized">Serialized COMMAND_REQ</param>
    /// <returns>Deserialized COMMAND_REQ</returns>
    public Result<CommandReqMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() =>
        {
            var commandReqMessageDto = Utils.FromProtobufs<CommandReqMessageDto>(serialized);

            var commandDeserialization =
                commandReqMessageDto.CommandReqType switch
                {
                    CommandReqTypes.INSERT => _insertCommandSerializer.FromDto(commandReqMessageDto.InsertCommandDto!).Map(cmd => (BaseCommand)cmd),
                    CommandReqTypes.UPDATE => _updateCommandSerializer.FromDto(commandReqMessageDto.UpdateCommandDto!).Map(cmd => (BaseCommand)cmd),
                    CommandReqTypes.DELETE => _deleteCommandSerializer.FromDto(commandReqMessageDto.DeleteCommandDto!).Map(cmd => (BaseCommand)cmd),
                    _ => throw new Exception("Unknown command type")
                };
            var commandCreation = commandDeserialization.Map(command =>
                new CommandReqMessage(
                    commandReqMessageDto.ExchangeId,
                    commandReqMessageDto.NodeId,
                    command
                    ));

            return commandCreation;
        });

    /// <summary>
    /// Serializes a COMMAND_REQ message
    /// </summary>
    /// <param name="message">COMMAND_REQ message to serialize</param>
    /// <returns>Serialized COMMAND_REQ</returns>
    public Result<byte[]> Serialize(CommandReqMessage message)
        => Results.AsResult(() =>
        {
            var deleteCommand =
                message.CommandReqType == CommandReqTypes.DELETE
                ? _deleteCommandSerializer.ToDto((DeleteCommand)message.Command).Data
                : null;
            var insertCommand =
                message.CommandReqType == CommandReqTypes.INSERT
                ? _insertCommandSerializer.ToDto((InsertCommand)message.Command).Data
                : null;
            var updateCommand =
                message.CommandReqType == CommandReqTypes.UPDATE
                ? _updateCommandSerializer.ToDto((UpdateCommand)message.Command).Data
                : null;

            var commandReqMessageDto = new CommandReqMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                CommandReqType = message.CommandReqType,
                DeleteCommandDto = deleteCommand,
                InsertCommandDto = insertCommand,
                UpdateCommandDto = updateCommand
            };

            var messageBytes = Utils.ToProtobufs(commandReqMessageDto);

            return messageBytes;
        });
}
