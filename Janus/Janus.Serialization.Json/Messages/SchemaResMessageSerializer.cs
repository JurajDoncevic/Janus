using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Json.Messages.DTOs;
using Janus.Serialization.Json.SchemaModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Json.Messages;

/// <summary>
/// JSON format SCHEMA_RES message serializer
/// </summary>
public class SchemaResMessageSerializer : IMessageSerializer<SchemaResMessage, string>
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
    public Result<SchemaResMessage> Deserialize(string serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<SchemaResMessageDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize message DTO"))
            .Bind(schemaResMessageDto
                => _dataSourceSerializer.FromDto(schemaResMessageDto.DataSource)
                    .Map(dataSource
                        => new SchemaResMessage(
                        schemaResMessageDto.ExchangeId,
                        schemaResMessageDto.NodeId,
                        dataSource)));

    /// <summary>
    /// Serializes a SCHEMA_RES message
    /// </summary>
    /// <param name="message">SCHEMA_RES message to serialize</param>
    /// <returns>Serialized SCHEMA_RES</returns>
    public Result<string> Serialize(SchemaResMessage message)
        => Results.AsResult(() =>
        {
            var dataSourceDto = _dataSourceSerializer.ToDto(message.DataSource).Data!;

            var schemaResMessageDto = new SchemaResMessageDto
            {
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                DataSource = dataSourceDto
            };
            var json = JsonSerializer.Serialize(schemaResMessageDto, _serializerOptions);

            return json;
        });
}
