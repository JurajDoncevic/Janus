using FunctionalExtensions.Base.Resulting;
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
        => Results.AsResult(() =>
            JsonSerializer.Deserialize<BaseMessage>(serialized) ?? throw new Exception("Failed to deserialize message DTO"));

    /// <summary>
    /// Serializes a base message
    /// </summary>
    /// <param name="message">Base message to serialize</param>
    /// <returns>Serialized base message</returns>
    public Result<byte[]> Serialize(BaseMessage message)
        => Results.AsResult(() => JsonSerializer.Serialize(message))
            .Map(Encoding.UTF8.GetBytes);
}
