using Janus.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Bson.Messages.DTOs;
using System.Text;
using System.Text.Json;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format COMMAND_RES message serializer
/// </summary>
public sealed class CommandResMessageSerializer : IMessageSerializer<CommandResMessage, byte[]>
{
    /// <summary>
    /// Deserializes a COMMAND_RES message
    /// </summary>
    /// <param name="serialized">Serialized COMMAND_RES</param>
    /// <returns>Deserialized COMMAND_RES</returns>
    public Result<CommandResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<CommandResMessageDto>(serialized) ?? throw new Exception("Failed to deserialize message DTO"))
            .Map(commandResMessageDto =>
                new CommandResMessage(
                    commandResMessageDto.ExchangeId,
                    commandResMessageDto.NodeId,
                    commandResMessageDto.IsSuccess,
                    commandResMessageDto.OutcomeDescription));

    /// <summary>
    /// Serializes a COMMAND_RES message
    /// </summary>
    /// <param name="message">COMMAND_RES message to serialize</param>
    /// <returns>Serialized COMMAND_RES</returns>
    public Result<byte[]> Serialize(CommandResMessage message)
        => Results.AsResult(() =>
        {
            var commandResMessageDto = new CommandResMessageDto
            {
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                IsSuccess = message.IsSuccess,
                OutcomeDescription = message.OutcomeDescription
            };

            var json = JsonSerializer.Serialize(commandResMessageDto);
            var messageBytes = Encoding.UTF8.GetBytes(json);

            return messageBytes;
        });
}
