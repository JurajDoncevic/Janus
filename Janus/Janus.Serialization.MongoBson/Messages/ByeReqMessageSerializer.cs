using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.Messages.DTOs;

namespace Janus.Serialization.MongoBson.Messages;

/// <summary>
/// MongoBson format BYE_REQ message serializer
/// </summary>
public sealed class ByeReqMessageSerializer : IMessageSerializer<ByeReqMessage, byte[]>
{
    /// <summary>
    /// Deserializes a BYE_REQ message
    /// </summary>
    /// <param name="serialized">Serialized BYE_REQ</param>
    /// <returns>Deserialized BYE_REQ</returns>
    public Result<ByeReqMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromBson<ByeReqMessageDto>(serialized))
            .Map(byeReqMessageDto => new ByeReqMessage(byeReqMessageDto.ExchangeId, byeReqMessageDto.NodeId));

    /// <summary>
    /// Serializes a BYE_REQ message
    /// </summary>
    /// <param name="message">BYE_REQ message to serialize</param>
    /// <returns>Serialized BYE_REQ</returns>
    public Result<byte[]> Serialize(ByeReqMessage message)
        => Results.AsResult(()
            => Utils.ToBson(new ByeReqMessageDto
            {
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                Preamble = message.Preamble
            }));
}
