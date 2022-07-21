using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;

/// <summary>
/// Avro format HELLO_REQ message serializer
/// </summary>
public class HelloReqMessageSerializer : IMessageSerializer<HelloReqMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(HelloReqMessageDto));

    /// <summary>
    /// Deserializes a HELLO_REQ message
    /// </summary>
    /// <param name="serialized">Serialized HELLO_REQ</param>
    /// <returns>Deserialized HELLO_REQ</returns>
    public Result<HelloReqMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            var deserializedModel = AvroConvert.DeserializeHeadless<HelloReqMessageDto>(serialized, _schema);

            return new HelloReqMessage(
                deserializedModel.ExchangeId,
                deserializedModel.NodeId,
                deserializedModel.ListenPort,
                deserializedModel.NodeType,
                deserializedModel.RememberMe);
        });

    /// <summary>
    /// Serializes a HELLO_REQ message
    /// </summary>
    /// <param name="message">HELLO_REQ message to serialize</param>
    /// <returns>Serialized HELLO_REQ</returns>
    public Result<byte[]> Serialize(HelloReqMessage message)
        => ResultExtensions.AsResult(() =>
        {
            return AvroConvert.SerializeHeadless(message, _schema);
        });
        
}
