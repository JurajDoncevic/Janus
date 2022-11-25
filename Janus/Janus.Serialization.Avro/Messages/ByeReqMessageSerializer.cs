using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;

/// <summary>
/// Avro format BYE_REQ message serializer
/// </summary>
public sealed class ByeReqMessageSerializer : IMessageSerializer<ByeReqMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(ByeReqMessageDto));

    /// <summary>
    /// Deserializes a BYE_REQ message
    /// </summary>
    /// <param name="serialized">Serialized BYE_REQ</param>
    /// <returns>Deserialized BYE_REQ</returns>
    public Result<ByeReqMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => AvroConvert.DeserializeHeadless<ByeReqMessageDto>(serialized, _schema))
            .Map(byeReqMessageDto => new ByeReqMessage(byeReqMessageDto.ExchangeId, byeReqMessageDto.NodeId));

    /// <summary>
    /// Serializes a BYE_REQ message
    /// </summary>
    /// <param name="message">BYE_REQ message to serialize</param>
    /// <returns>Serialized BYE_REQ</returns>
    public Result<byte[]> Serialize(ByeReqMessage message)
        => Results.AsResult(()
            => AvroConvert.SerializeHeadless(message, _schema));
}
