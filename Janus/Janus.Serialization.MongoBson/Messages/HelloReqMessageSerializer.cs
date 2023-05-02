using Janus.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.Messages.DTOs;

namespace Janus.Serialization.MongoBson.Messages;

/// <summary>
/// MongoBson format HELLO_REQ message serializer
/// </summary>
public sealed class HelloReqMessageSerializer : IMessageSerializer<HelloReqMessage, byte[]>
{

    /// <summary>
    /// Deserializes a HELLO_REQ message
    /// </summary>
    /// <param name="serialized">Serialized HELLO_REQ</param>
    /// <returns>Deserialized HELLO_REQ</returns>
    public Result<HelloReqMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromBson<HelloReqMessageDto>(serialized))
            .Map(helloReqMessageDto =>
                new HelloReqMessage(
                    helloReqMessageDto.ExchangeId,
                    helloReqMessageDto.NodeId,
                    helloReqMessageDto.ListenPort,
                    helloReqMessageDto.NodeType,
                    helloReqMessageDto.RememberMe));

    /// <summary>
    /// Serializes a HELLO_REQ message
    /// </summary>
    /// <param name="message">HELLO_REQ message to serialize</param>
    /// <returns>Serialized HELLO_REQ</returns>
    public Result<byte[]> Serialize(HelloReqMessage message)
        => Results.AsResult(() =>
        {
            var helloReqMessageDto = new HelloReqMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                ListenPort = message.ListenPort,
                NodeType = message.NodeType,
                RememberMe = message.RememberMe
            };
            return Utils.ToBson(helloReqMessageDto);
        });

}
