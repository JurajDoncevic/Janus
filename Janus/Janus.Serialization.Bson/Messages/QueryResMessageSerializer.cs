using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Bson.DataModels;
using Janus.Serialization.Bson.Messages.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format QUERY_RES message serializer
/// </summary>
public class QueryResMessageSerializer : IMessageSerializer<QueryResMessage, byte[]>
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
    public Result<QueryResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => JsonSerializer.Deserialize<QueryResMessageDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize message DTO"))
            .Bind(queryResMessageDto =>
                _tabularDataSerializer.FromDto(queryResMessageDto.TabularData)
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
    public Result<byte[]> Serialize(QueryResMessage message)
        => Results.AsResult(() =>
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
            var messageBytes = Encoding.UTF8.GetBytes(json);

            return messageBytes;
        });
}
