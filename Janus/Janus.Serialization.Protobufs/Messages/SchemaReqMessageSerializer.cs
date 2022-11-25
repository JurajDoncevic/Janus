using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Protobufs.Messages.DTOs;

namespace Janus.Serialization.Protobufs.Messages;

/// <summary>
/// Protobufs format SCHEMA_REQ message serializer
/// </summary>
public sealed class SchemaReqMessageSerializer : IMessageSerializer<SchemaReqMessage, byte[]>
{
    /// <summary>
    /// Deserializes a SCHEMA_REQ message
    /// </summary>
    /// <param name="serialized">Serialized SCHEMA_REQ</param>
    /// <returns>Deserialized SCHEMA_REQ</returns>
    public Result<SchemaReqMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromProtobufs<SchemaReqMessageDto>(serialized))
            .Bind<SchemaReqMessageDto, SchemaReqMessage>(schemaReqMessageDto => new SchemaReqMessage(schemaReqMessageDto.ExchangeId, schemaReqMessageDto.NodeId));


    /// <summary>
    /// Serializes a SCHEMA_REQ message
    /// </summary>
    /// <param name="message">SCHEMA_REQ message to serialize</param>
    /// <returns>Serialized SCHEMA_REQ</returns>
    public Result<byte[]> Serialize(SchemaReqMessage message)
        => Results.AsResult(() =>
        {
            var shemaReqMessageDto = new SchemaReqMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId
            };
            return Utils.ToProtobufs(shemaReqMessageDto);
        });
}
