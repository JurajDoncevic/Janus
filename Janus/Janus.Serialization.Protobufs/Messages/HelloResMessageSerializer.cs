using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Protobufs.Messages.DTOs;

namespace Janus.Serialization.Protobufs.Messages;

/// <summary>
/// Protobufs format HELLO_RES message serializer
/// </summary>
public class HelloResMessageSerializer : IMessageSerializer<HelloResMessage, byte[]>
{

    /// <summary>
    /// Deserializes a HELLO_RES message
    /// </summary>
    /// <param name="serialized">Serialized HELLO_RES</param>
    /// <returns>Deserialized HELLO_RES</returns>
    public Result<HelloResMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            var deserializedModel = Utils.FromProtobufs<HelloResMessageDto>(serialized);

            return new HelloResMessage(
                deserializedModel.ExchangeId,
                deserializedModel.NodeId,
                deserializedModel.ListenPort,
                deserializedModel.NodeType,
                deserializedModel.RememberMe,
                deserializedModel.ContextMessage);
        });

    /// <summary>
    /// Serializes a HELLO_RES message
    /// </summary>
    /// <param name="message">HELLO_RES message to serialize</param>
    /// <returns>Serialized HELLO_RES</returns>
    public Result<byte[]> Serialize(HelloResMessage message)
        => ResultExtensions.AsResult(() =>
        {
            var helloResMessageDto = new HelloResMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                ListenPort = message.ListenPort,
                NodeType = message.NodeType,
                RememberMe = message.RememberMe,
                ContextMessage = message.ContextMessage
            };
            return Utils.ToProtobufs(helloResMessageDto);
        });
}
