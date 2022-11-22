using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using Janus.Serialization.Avro.SchemaModels;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;

/// <summary>
/// Avro format SCHEMA_RES message serializer
/// </summary>
public class SchemaResMessageSerializer : IMessageSerializer<SchemaResMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(SchemaResMessageDto));
    private readonly DataSourceSerializer _dataSourceSerializer = new DataSourceSerializer();

    /// <summary>
    /// Deserializes a SCHEMA_RES message
    /// </summary>
    /// <param name="serialized">Serialized SCHEMA_RES</param>
    /// <returns>Deserialized SCHEMA_RES</returns>
    public Result<SchemaResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => AvroConvert.DeserializeHeadless<SchemaResMessageDto>(serialized, _schema))
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
            return AvroConvert.SerializeHeadless(dto, _schema);
        });
}
