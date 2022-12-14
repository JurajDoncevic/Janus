using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.Messages.DTOs;
using Janus.Serialization.MongoBson.SchemaModels;
using System.Text.Json;

namespace Janus.Serialization.MongoBson.Messages;

/// <summary>
/// MongoBson format SCHEMA_RES message serializer
/// </summary>
public sealed class SchemaResMessageSerializer : IMessageSerializer<SchemaResMessage, byte[]>
{
    private readonly DataSourceSerializer _dataSourceSerializer = new DataSourceSerializer();

    /// <summary>
    /// Deserializes a SCHEMA_RES message
    /// </summary>
    /// <param name="serialized">Serialized SCHEMA_RES</param>
    /// <returns>Deserialized SCHEMA_RES</returns>
    public Result<SchemaResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromBson<SchemaResMessageDto>(serialized))
            .Bind(schemaResMessageDto =>
                    schemaResMessageDto.DataSource is not null
                    ? _dataSourceSerializer.FromDto(schemaResMessageDto.DataSource)
                                           .Map(dataSource =>
                                               new SchemaResMessage(
                                                   schemaResMessageDto.ExchangeId,
                                                   schemaResMessageDto.NodeId,
                                                   dataSource))
                    : Results.AsResult(() => new SchemaResMessage(schemaResMessageDto.ExchangeId,
                                                                  schemaResMessageDto.NodeId,
                                                                  null,
                                                                  schemaResMessageDto.OutcomeDescription)));

    /// <summary>
    /// Serializes a SCHEMA_RES message
    /// </summary>
    /// <param name="message">SCHEMA_RES message to serialize</param>
    /// <returns>Serialized SCHEMA_RES</returns>
    public Result<byte[]> Serialize(SchemaResMessage message)
        => Results.AsResult(() =>
        {
            var serialization = message.DataSource.Match(
                dataSource => _dataSourceSerializer.ToDto(dataSource)
                              .Map(dataSourceDto => new SchemaResMessageDto
                              {
                                  Preamble = message.Preamble,
                                  ExchangeId = message.ExchangeId,
                                  NodeId = message.NodeId,
                                  OutcomeDescription = message.OutcomeDescription,
                                  DataSource = dataSourceDto
                              }),
                () => Results.AsResult(() => new SchemaResMessageDto
                {
                    Preamble = message.Preamble,
                    ExchangeId = message.ExchangeId,
                    NodeId = message.NodeId,
                    OutcomeDescription = message.OutcomeDescription,
                    DataSource = null
                })
                ).Bind(dto => Results.AsResult(() => Utils.ToBson(dto)));

            return serialization;
        });
}
