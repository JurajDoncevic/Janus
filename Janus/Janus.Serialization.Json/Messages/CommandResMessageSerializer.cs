using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Json.Messages.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        => ResultExtensions.AsResult(() =>
        {
            var commandResMessageDto = JsonSerializer.Deserialize<CommandResMessageDto>(serialized);

            if (commandResMessageDto == null)
                throw new Exception("Failed to deserialize COMMAND_RES DTO");

            return new CommandResMessage(
                commandResMessageDto.ExchangeId,
                commandResMessageDto.NodeId,
                commandResMessageDto.IsSuccess,
                commandResMessageDto.OutcomeDescription
                );
        });

    /// <summary>
    /// Serializes a COMMAND_RES message
    /// </summary>
    /// <param name="message">COMMAND_RES message to serialize</param>
    /// <returns>Serialized COMMAND_RES</returns>
    public Result<string> Serialize(CommandResMessage message)
        => ResultExtensions.AsResult(() =>
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
