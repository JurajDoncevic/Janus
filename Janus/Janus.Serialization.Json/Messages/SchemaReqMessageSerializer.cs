using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Json.Messages.DTOs;
using System.Text.Json;

namespace Janus.Serialization.Json.Messages;

/// <summary>
/// JSON format SCHEMA_REQ message serializer
/// </summary>
public class SchemaReqMessageSerializer : IMessageSerializer<SchemaReqMessage, string>
{
    /// <summary>
    /// Deserializes a SCHEMA_REQ message
    /// </summary>
    /// <param name="serialized">Serialized SCHEMA_REQ</param>
    /// <returns>Deserialized SCHEMA_REQ</returns>
    public Result<SchemaReqMessage> Deserialize(string serialized)
        => ResultExtensions.AsResult(() => JsonSerializer.Deserialize<SchemaReqMessageDto>(serialized) ?? throw new Exception("Failed to deserialize message DTO"))
            .Map(schemaReqMessageDto
                => new SchemaReqMessage(
                    schemaReqMessageDto.ExchangeId,
                    schemaReqMessageDto.NodeId));

    /// <summary>
    /// Serializes a SCHEMA_REQ message
    /// </summary>
    /// <param name="message">SCHEMA_REQ message to serialize</param>
    /// <returns>Serialized SCHEMA_REQ</returns>
    public Result<string> Serialize(SchemaReqMessage message)
        => ResultExtensions.AsResult(() =>
        {
            var schemaReqMessageDto = new SchemaReqMessageDto
            {
                NodeId = message.NodeId,
                ExchangeId = message.ExchangeId
            };
            var json = JsonSerializer.Serialize(schemaReqMessageDto);

            return json;
        });
}
