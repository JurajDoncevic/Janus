using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;

/// <summary>
/// Avro base message serializer
/// </summary>
public class BaseMessageSerializer : IMessageSerializer<BaseMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(BaseMessageDto));

    /// <summary>
    /// Deserializes a base message
    /// </summary>
    /// <param name="serialized">Serialized base message</param>
    /// <returns>Deserialized base message</returns>
    public Result<BaseMessage> Deserialize(byte[] serialized)
        => Results.AsResult(()
            => AvroConvert.DeserializeHeadless<BaseMessage>(serialized, _schema)
        );

    /// <summary>
    /// Serializes a base message
    /// </summary>
    /// <param name="message">Base message to serialize</param>
    /// <returns>Serialized base message</returns>
    public Result<byte[]> Serialize(BaseMessage message)
        => Results.AsResult(()
            => AvroConvert.SerializeHeadless(message, _schema)
        );
}
