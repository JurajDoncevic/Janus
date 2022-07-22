using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Json.DataModels;
using Janus.Serialization.Json.Messages.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Json.Messages;

/// <summary>
/// JSON format QUERY_RES message serializer
/// </summary>
public class QueryResMessageSerializer : IMessageSerializer<QueryResMessage, string>
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly TabularDataSerializer _tabularDataSerializer = new TabularDataSerializer();

    public QueryResMessageSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a QUERY_RES message
    /// </summary>
    /// <param name="serialized">Serialized QUERY_RES</param>
    /// <returns>Deserialized QUERY_RES</returns>
    public Result<QueryResMessage> Deserialize(string serialized)
        => ResultExtensions.AsResult(() => JsonSerializer.Deserialize<QueryResMessageDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize message DTO"))
            .Bind(queryResMessageDto => _tabularDataSerializer.FromDto(queryResMessageDto.TabularData)
                    .Map(tabularData =>
                        new QueryResMessage(
                            queryResMessageDto.ExchangeId,
                            queryResMessageDto.NodeId,
                            tabularData,
                            queryResMessageDto.ErrorMessage,
                            queryResMessageDto.BlockNumber,
                            queryResMessageDto.TotalBlocks)));

    /// <summary>
    /// Serializes a QUERY_RES message
    /// </summary>
    /// <param name="message">QUERY_RES message to serialize</param>
    /// <returns>Serialized QUERY_RES</returns>
    public Result<string> Serialize(QueryResMessage message)
        => ResultExtensions.AsResult(() =>
        {
            var tabularDataDto = _tabularDataSerializer.ToDto(message.TabularData).Data;
            var queryResMessageDto = new QueryResMessageDto
            {
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                TabularData = tabularDataDto,
                BlockNumber = message.BlockNumber,
                ErrorMessage = message.ErrorMessage,
                TotalBlocks = message.TotalBlocks
            };

            var json = JsonSerializer.Serialize(queryResMessageDto, _serializerOptions);

            return json;
        });
}
