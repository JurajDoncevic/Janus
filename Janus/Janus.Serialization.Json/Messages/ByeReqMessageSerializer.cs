using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Json.Messages.DTOs;
using System.Text.Json;

namespace Janus.Serialization.Json.Messages;

/// <summary>
/// JSON format BYE_REQ message serializer
/// </summary>
public class ByeReqMessageSerializer : IMessageSerializer<ByeReqMessage, string>
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
    public Result<ByeReqMessage> Deserialize(string serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<ByeReqMessageDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize message DTO"))
            .Map(byeReqMessageDto
                => new ByeReqMessage(
                    byeReqMessageDto.ExchangeId,
                    byeReqMessageDto.NodeId));

    /// <summary>
    /// Serializes a BYE_REQ message
    /// </summary>
    /// <param name="message">BYE_REQ message to serialize</param>
    /// <returns>Serialized BYE_REQ</returns>
    public Result<string> Serialize(ByeReqMessage message)
        => Results.AsResult(() =>
        {
            var byeReqMessageDto = new ByeReqMessageDto
            {
                NodeId = message.NodeId,
                ExchangeId = message.ExchangeId
            };
            var json = JsonSerializer.Serialize(byeReqMessageDto, _serializerOptions);

            return json;
        });
}
