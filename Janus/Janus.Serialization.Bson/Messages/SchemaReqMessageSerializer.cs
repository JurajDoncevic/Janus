using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Bson.Messages.DTOs;
using System.Text;
using System.Text.Json;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format SCHEMA_REQ message serializer
/// </summary>
public sealed class SchemaReqMessageSerializer : IMessageSerializer<SchemaReqMessage, byte[]>
{
    /// <summary>
    /// Deserializes a SCHEMA_REQ message
    /// </summary>
    /// <param name="serialized">Serialized SCHEMA_REQ</param>
    /// <returns>Deserialized SCHEMA_REQ</returns>
    public Result<SchemaReqMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<SchemaReqMessageDto>(serialized) ?? throw new Exception("Failed to deserialize message DTO"))
            .Map(schemaReqMessageDto =>
                new SchemaReqMessage(
                    schemaReqMessageDto.ExchangeId,
                    schemaReqMessageDto.NodeId));

    /// <summary>
    /// Serializes a SCHEMA_REQ message
    /// </summary>
    /// <param name="message">SCHEMA_REQ message to serialize</param>
    /// <returns>Serialized SCHEMA_REQ</returns>
    public Result<byte[]> Serialize(SchemaReqMessage message)
        => Results.AsResult(() =>
        {
            var schemaReqMessageDto = new SchemaReqMessageDto
            {
                NodeId = message.NodeId,
                ExchangeId = message.ExchangeId
            };
            var json = JsonSerializer.Serialize(schemaReqMessageDto);
            var messageBytes = Encoding.UTF8.GetBytes(json);

            return messageBytes;
        });
}
