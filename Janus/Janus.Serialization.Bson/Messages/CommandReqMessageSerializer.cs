using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Commons.Messages;
using Janus.Serialization.Bson.CommandModels;
using Janus.Serialization.Bson.Messages.DTOs;
using Janus.Serialization.Bson.QueryModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format COMMAND_REQ message serializer
/// </summary>
public sealed class CommandReqMessageSerializer : IMessageSerializer<CommandReqMessage, byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly DeleteCommandSerializer _deleteCommandSerializer = new DeleteCommandSerializer();
    private readonly InsertCommandSerializer _insertCommandSerializer = new InsertCommandSerializer();
    private readonly UpdateCommandSerializer _updateCommandSerializer = new UpdateCommandSerializer();

    public CommandReqMessageSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new SelectionExpressionJsonConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a COMMAND_REQ message
    /// </summary>
    /// <param name="serialized">Serialized COMMAND_REQ</param>
    /// <returns>Deserialized COMMAND_REQ</returns>
    public Result<CommandReqMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() =>
        {
            var commandReqMessageDto = JsonSerializer.Deserialize<CommandReqMessageDto>(serialized, _serializerOptions);

            if (commandReqMessageDto == null)
                throw new Exception("Failed to deserialize COMMAND_REQ DTO");

            var commandDeserialization =
                commandReqMessageDto.CommandReqType switch
                {
                    CommandReqTypes.INSERT => _insertCommandSerializer.FromDto(commandReqMessageDto.InsertCommandDto!)
                                                                      .Map(cmd => (BaseCommand)cmd),
                    CommandReqTypes.UPDATE => _updateCommandSerializer.FromDto(commandReqMessageDto.UpdateCommandDto!)
                                                                      .Map(cmd => (BaseCommand)cmd),
                    CommandReqTypes.DELETE => _deleteCommandSerializer.FromDto(commandReqMessageDto.DeleteCommandDto!)
                                                                      .Map(cmd => (BaseCommand)cmd),
                    _ => throw new Exception("Unknown command type")
                };
            var commandReqMessageCreation = commandDeserialization.Map(command =>
                new CommandReqMessage(
                    commandReqMessageDto.ExchangeId,
                    commandReqMessageDto.NodeId,
                    command
                    ));

            return commandReqMessageCreation.Data!;
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
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                CommandReqType = message.CommandReqType,
                DeleteCommandDto = deleteCommand,
                InsertCommandDto = insertCommand,
                UpdateCommandDto = updateCommand
            };

            var json = JsonSerializer.Serialize(commandReqMessageDto, _serializerOptions);
            var bson = Encoding.UTF8.GetBytes(json);

            return bson;
        });
}
