using Janus.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Json.Messages.DTOs;
using System.Text.Json;

namespace Janus.Serialization.Json.Messages;

/// <summary>
/// JSON format COMMAND_RES message serializer
/// </summary>
public class CommandResMessageSerializer : IMessageSerializer<CommandResMessage, string>
{
    /// <summary>
    /// Deserializes a COMMAND_RES message
    /// </summary>
    /// <param name="serialized">Serialized COMMAND_RES</param>
    /// <returns>Deserialized COMMAND_RES</returns>
    public Result<CommandResMessage> Deserialize(string serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<CommandResMessageDto>(serialized) ?? throw new Exception("Failed to deserialize message DTO"))
            .Map(commandResMessageDto
                => new CommandResMessage(
                    commandResMessageDto.ExchangeId,
                    commandResMessageDto.NodeId,
                    commandResMessageDto.IsSuccess,
                    commandResMessageDto.OutcomeDescription));

    /// <summary>
    /// Serializes a COMMAND_RES message
    /// </summary>
    /// <param name="message">COMMAND_RES message to serialize</param>
    /// <returns>Serialized COMMAND_RES</returns>
    public Result<string> Serialize(CommandResMessage message)
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

            return json;
        });
}
