using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Bson.Messages.DTOs;
using Janus.Serialization.Bson.QueryModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format QUERY_REQ message serializer
/// </summary>
public class QueryReqMessageSerializer : IMessageSerializer<QueryReqMessage, byte[]>
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly QuerySerializer _querySerializer = new QuerySerializer();

    public QueryReqMessageSerializer()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new SelectionExpressionJsonConverter());

        _serializerOptions = options;
    }

    /// <summary>
    /// Deserializes a QUERY_REQ message
    /// </summary>
    /// <param name="serialized">Serialized QUERY_REQ</param>
    /// <returns>Deserialized QUERY_REQ</returns>
    public Result<QueryReqMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            var queryReqMessageDto = JsonSerializer.Deserialize<QueryReqMessageDto>(serialized, _serializerOptions);

            if (queryReqMessageDto == null)
                throw new Exception("Failed to deserialize QUERY_REQ DTO");

            var query = _querySerializer.FromDto(queryReqMessageDto.Query).Data!;

            return new QueryReqMessage(
                queryReqMessageDto.ExchangeId,
                queryReqMessageDto.NodeId,
                query
                );
        });

    /// <summary>
    /// Serializes a QUERY_REQ message
    /// </summary>
    /// <param name="message">QUERY_REQ message to serialize</param>
    /// <returns>Serialized QUERY_REQ</returns>
    public Result<byte[]> Serialize(QueryReqMessage message)
        => ResultExtensions.AsResult(() =>
        {
            var queryDto = _querySerializer.ToDto(message.Query).Data!;

            var queryReqMessageDto = new QueryReqMessageDto
            {
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                Query = queryDto
            };

            var json = JsonSerializer.Serialize(queryReqMessageDto, _serializerOptions);
            var messageBytes = Encoding.UTF8.GetBytes(json);

            return messageBytes;
        });
}
