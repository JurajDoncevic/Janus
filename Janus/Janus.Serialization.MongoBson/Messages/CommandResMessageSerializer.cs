using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.Messages.DTOs;

namespace Janus.Serialization.MongoBson.Messages;


/// <summary>
/// MongoBson format COMMAND_RES message serializer
/// </summary>
public class CommandResMessageSerializer : IMessageSerializer<CommandResMessage, byte[]>
{
    /// <summary>
    /// Deserializes a COMMAND_RES message
    /// </summary>
    /// <param name="serialized">Serialized COMMAND_RES</param>
    /// <returns>Deserialized COMMAND_RES</returns>
    public Result<CommandResMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() => Utils.FromBson<CommandResMessageDto>(serialized))
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

            return Utils.ToBson(commandMessageDto);
        });
}
