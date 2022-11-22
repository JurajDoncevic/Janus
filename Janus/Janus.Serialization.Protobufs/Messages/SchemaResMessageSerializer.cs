using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Protobufs.Messages.DTOs;
using Janus.Serialization.Protobufs.SchemaModels;

namespace Janus.Serialization.Protobufs.Messages;

/// <summary>
/// Protobufs format SCHEMA_RES message serializer
/// </summary>
public class SchemaResMessageSerializer : IMessageSerializer<SchemaResMessage, byte[]>
{
    private readonly DataSourceSerializer _dataSourceSerializer = new DataSourceSerializer();

    /// <summary>
    /// Deserializes a SCHEMA_RES message
    /// </summary>
    /// <param name="serialized">Serialized SCHEMA_RES</param>
    /// <returns>Deserialized SCHEMA_RES</returns>
    public Result<SchemaResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromProtobufs<SchemaResMessageDto>(serialized))
            .Bind(schemaResMessageDto =>
                _dataSourceSerializer.FromDto(schemaResMessageDto.DataSource)
                    .Map(dataSource =>
                        new SchemaResMessage(
                            schemaResMessageDto.ExchangeId,
                            schemaResMessageDto.NodeId,
                            dataSource)));

    /// <summary>
    /// Serializes a SCHEMA_RES message
    /// </summary>
    /// <param name="message">SCHEMA_RES message to serialize</param>
    /// <returns>Serialized SCHEMA_RES</returns>
    public Result<byte[]> Serialize(SchemaResMessage message)
        => Results.AsResult(() =>
        {
            var dto = new SchemaResMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                DataSource = _dataSourceSerializer.ToDto(message.DataSource).Data!
            };
            return Utils.ToProtobufs(dto);
        });
}
