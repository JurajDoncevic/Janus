using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Bson.Messages.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format HELLO_REQ message serializer
/// </summary>
public class HelloReqMessageSerializer : IMessageSerializer<HelloReqMessage, byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public HelloReqMessageSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a HELLO_REQ message
    /// </summary>
    /// <param name="serialized">Serialized HELLO_REQ</param>
    /// <returns>Deserialized HELLO_REQ</returns>
    public Result<HelloReqMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() => JsonSerializer.Deserialize<HelloReqMessageDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize message DTO"))
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
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                ListenPort = message.ListenPort,
                NodeType = message.NodeType,
                RememberMe = message.RememberMe
            };
            var json = JsonSerializer.Serialize(helloReqMessageDto, _serializerOptions);
            var messageBytes = Encoding.UTF8.GetBytes(json);

            return messageBytes;
        });
}
