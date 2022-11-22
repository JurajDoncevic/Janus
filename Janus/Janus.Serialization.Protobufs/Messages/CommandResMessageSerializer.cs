using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Protobufs.Messages.DTOs;

namespace Janus.Serialization.Protobufs.Messages;


/// <summary>
/// Protobufs format COMMAND_RES message serializer
/// </summary>
public class CommandResMessageSerializer : IMessageSerializer<CommandResMessage, byte[]>
{
    /// <summary>
    /// Deserializes a COMMAND_RES message
    /// </summary>
    /// <param name="serialized">Serialized COMMAND_RES</param>
    /// <returns>Deserialized COMMAND_RES</returns>
    public Result<CommandResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromProtobufs<CommandResMessageDto>(serialized))
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
            var commandMessageDto = new CommandResMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                IsSuccess = message.IsSuccess,
                OutcomeDescription = message.OutcomeDescription
            };

            return Utils.ToProtobufs(commandMessageDto);
        });
}
