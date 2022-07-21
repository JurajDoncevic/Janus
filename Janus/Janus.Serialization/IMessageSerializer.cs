using Janus.Commons.Messages;

namespace Janus.Serialization;

/// <summary>
/// Serializer interface for message types
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TSerialized"></typeparam>
public interface IMessageSerializer<TMessage, TSerialized> where TMessage : BaseMessage
{
    /// <summary>
    /// Serializes a message
    /// </summary>
    /// <param name="message">Message to serialize</param>
    /// <returns>Serialized message</returns>
    public Result<TSerialized> Serialize(TMessage message);

    /// <summary>
    /// Deserializes a message
    /// </summary>
    /// <param name="serialized">Message to deserialize</param>
    /// <returns>Deserialized message</returns>
    public Result<TMessage> Deserialize(TSerialized serialized);
}
