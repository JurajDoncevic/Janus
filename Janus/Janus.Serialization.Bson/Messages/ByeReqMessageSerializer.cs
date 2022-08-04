using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Bson.Messages.DTOs;
using System.Text;
using System.Text.Json;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format BYE_REQ message serializer
/// </summary>
public class ByeReqMessageSerializer : IMessageSerializer<ByeReqMessage, byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public ByeReqMessageSerializer()
    {
        var options = new JsonSerializerOptions();

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a BYE_REQ message
    /// </summary>
    /// <param name="serialized">Serialized BYE_REQ</param>
    /// <returns>Deserialized BYE_REQ</returns>
    public Result<ByeReqMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() => JsonSerializer.Deserialize<ByeReqMessageDto>(serialized) ?? throw new Exception("Failed to deserialize message DTO"))
                .Map(byeReqMessageDto =>
                    new ByeReqMessage(
                        byeReqMessageDto.ExchangeId,
                        byeReqMessageDto.NodeId));

    /// <summary>
    /// Serializes a BYE_REQ message
    /// </summary>
    /// <param name="message">BYE_REQ message to serialize</param>
    /// <returns>Serialized BYE_REQ</returns>
    public Result<byte[]> Serialize(ByeReqMessage message)
        => ResultExtensions.AsResult(() =>
        {
            var byeReqMessageDto = new ByeReqMessageDto
            {
                NodeId = message.NodeId,
                ExchangeId = message.ExchangeId
            };
            var json = JsonSerializer.Serialize(byeReqMessageDto, _serializerOptions);
            var messageBytes = Encoding.UTF8.GetBytes(json);

            return messageBytes;
        });
}
