using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;

/// <summary>
/// Avro format HELLO_RES message serializer
/// </summary>
public sealed class HelloResMessageSerializer : IMessageSerializer<HelloResMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(HelloResMessageDto));

    /// <summary>
    /// Deserializes a HELLO_RES message
    /// </summary>
    /// <param name="serialized">Serialized HELLO_RES</param>
    /// <returns>Deserialized HELLO_RES</returns>
    public Result<HelloResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() =>
        {
            var deserializedModel = AvroConvert.DeserializeHeadless<HelloResMessageDto>(serialized, _schema);

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
        => Results.AsResult(() =>
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
            return AvroConvert.SerializeHeadless(helloResMessageDto, _schema);
        });
}
