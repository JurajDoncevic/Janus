﻿using Janus.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Commons.SchemaModels;
using Janus.Serialization.Bson.Messages.DTOs;
using Janus.Serialization.Bson.SchemaModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format SCHEMA_RES message serializer
/// </summary>
public sealed class SchemaResMessageSerializer : IMessageSerializer<SchemaResMessage, byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly DataSourceSerializer _dataSourceSerializer = new DataSourceSerializer();

    public SchemaResMessageSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a SCHEMA_RES message
    /// </summary>
    /// <param name="serialized">Serialized SCHEMA_RES</param>
    /// <returns>Deserialized SCHEMA_RES</returns>
    public Result<SchemaResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<SchemaResMessageDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize message DTO"))
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
                                  ExchangeId = message.ExchangeId,
                                  NodeId = message.NodeId,
                                  OutcomeDescription = message.OutcomeDescription,
                                  DataSource = dataSourceDto
                              }),
                () => Results.AsResult(() => new SchemaResMessageDto
                {
                    ExchangeId = message.ExchangeId,
                    NodeId = message.NodeId,
                    OutcomeDescription = message.OutcomeDescription,
                    DataSource = null
                })
                ).Bind(dto => Results.AsResult(() =>
                {
                    var json = JsonSerializer.Serialize(dto, _serializerOptions);
                    var messageBytes = Encoding.UTF8.GetBytes(json);
                    return messageBytes;
                }));

            return serialization;
        });
}
