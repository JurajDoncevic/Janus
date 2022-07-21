﻿using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;


/// <summary>
/// Avro format COMMAND_RES message serializer
/// </summary>
public class CommandResMessageSerializer : IMessageSerializer<CommandResMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(CommandResMessageDto));

    /// <summary>
    /// Deserializes a COMMAND_RES message
    /// </summary>
    /// <param name="serialized">Serialized COMMAND_RES</param>
    /// <returns>Deserialized COMMAND_RES</returns>
    public Result<CommandResMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            var commandResMessageDto = AvroConvert.DeserializeHeadless<CommandResMessageDto>(serialized, _schema);

            var commandResMessage = 
                new CommandResMessage(
                    commandResMessageDto.ExchangeId,
                    commandResMessageDto.NodeId,
                    commandResMessageDto.IsSuccess,
                    commandResMessageDto.OutcomeDescription);

            return commandResMessage;

        });


    /// <summary>
    /// Serializes a COMMAND_RES message
    /// </summary>
    /// <param name="message">COMMAND_RES message to serialize</param>
    /// <returns>Serialized COMMAND_RES</returns>
    public Result<byte[]> Serialize(CommandResMessage message)
        => ResultExtensions.AsResult(() =>
        {
            var commandMessageDto = new CommandResMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                IsSuccess = message.IsSuccess,
                OutcomeDescription = message.OutcomeDescription
            };

            var bytes = AvroConvert.SerializeHeadless(commandMessageDto, _schema);

            return bytes;
        });
}
