using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Bson.Messages.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format HELLO_RES message serializer
/// </summary>
public class HelloResMessageSerializer : IMessageSerializer<HelloResMessage, byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public HelloResMessageSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a HELLO_RES message
    /// </summary>
    /// <param name="serialized">Serialized HELLO_RES</param>
    /// <returns>Deserialized HELLO_RES</returns>
    public Result<HelloResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<HelloResMessageDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize message DTO"))
            .Map(helloResMessageDto =>
                new HelloResMessage(
                    helloResMessageDto.ExchangeId,
                    helloResMessageDto.NodeId,
                    helloResMessageDto.ListenPort,
                    helloResMessageDto.NodeType,
                    helloResMessageDto.RememberMe,
                    helloResMessageDto.ContextMessage));

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
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                ListenPort = message.ListenPort,
                NodeType = message.NodeType,
                RememberMe = message.RememberMe,
                ContextMessage = message.ContextMessage
            };
            var json = JsonSerializer.Serialize(helloResMessageDto, _serializerOptions);
            var messageBytes = Encoding.UTF8.GetBytes(json);

            return messageBytes;
        });
}
