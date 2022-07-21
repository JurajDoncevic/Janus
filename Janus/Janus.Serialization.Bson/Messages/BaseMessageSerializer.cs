using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using System.Text;
using System.Text.Json;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON base message serializer
/// </summary>
public class BaseMessageSerializer : IMessageSerializer<BaseMessage, byte[]>
{
    /// <summary>
    /// Deserializes a base message
    /// </summary>
    /// <param name="serialized">Serialized base message</param>
    /// <returns>Deserialized base message</returns>
    public Result<BaseMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            return JsonSerializer.Deserialize<BaseMessage>(serialized)!;
        });

    /// <summary>
    /// Serializes a base message
    /// </summary>
    /// <param name="message">Base message to serialize</param>
    /// <returns>Serialized base message</returns>
    public Result<byte[]> Serialize(BaseMessage message)
        => ResultExtensions.AsResult(() =>
        {
            var json = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(json);

            return messageBytes;
        });
}
