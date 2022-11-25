using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;

/// <summary>
/// Avro format SCHEMA_REQ message serializer
/// </summary>
public sealed class SchemaReqMessageSerializer : IMessageSerializer<SchemaReqMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(SchemaReqMessageDto));

    /// <summary>
    /// Deserializes a SCHEMA_REQ message
    /// </summary>
    /// <param name="serialized">Serialized SCHEMA_REQ</param>
    /// <returns>Deserialized SCHEMA_REQ</returns>
    public Result<SchemaReqMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => AvroConvert.DeserializeHeadless<SchemaReqMessageDto>(serialized, _schema))
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
            return AvroConvert.SerializeHeadless(shemaReqMessageDto, _schema);
        });
}
