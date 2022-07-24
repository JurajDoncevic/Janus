using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Protobufs.Messages.DTOs;

namespace Janus.Serialization.Protobufs.Messages;

/// <summary>
/// Protobufs format HELLO_REQ message serializer
/// </summary>
public class HelloReqMessageSerializer : IMessageSerializer<HelloReqMessage, byte[]>
{

    /// <summary>
    /// Deserializes a HELLO_REQ message
    /// </summary>
    /// <param name="serialized">Serialized HELLO_REQ</param>
    /// <returns>Deserialized HELLO_REQ</returns>
    public Result<HelloReqMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() => Utils.FromProtobufs<HelloReqMessageDto>(serialized))
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
        => ResultExtensions.AsResult(() =>
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
            return Utils.ToProtobufs(helloReqMessageDto);
        });
        
}
