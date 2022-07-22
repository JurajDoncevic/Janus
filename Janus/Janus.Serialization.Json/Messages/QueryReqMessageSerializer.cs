using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Json.Messages.DTOs;
using Janus.Serialization.Json.QueryModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Json.Messages;

/// <summary>
/// JSON format QUERY_REQ message serializer
/// </summary>
public class QueryReqMessageSerializer : IMessageSerializer<QueryReqMessage, string>
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
    public Result<QueryReqMessage> Deserialize(string serialized)
        => ResultExtensions.AsResult(() => JsonSerializer.Deserialize<QueryReqMessageDto>(serialized, _serializerOptions) ?? throw new Exception("Failed to deserialize message DTO"))
            .Bind(queryReqMessageDto 
                => _querySerializer.FromDto(queryReqMessageDto.Query)
                    .Map(query 
                        => new QueryReqMessage(
                            queryReqMessageDto.ExchangeId,
                            queryReqMessageDto.NodeId,
                            query)));

    /// <summary>
    /// Serializes a QUERY_REQ message
    /// </summary>
    /// <param name="message">QUERY_REQ message to serialize</param>
    /// <returns>Serialized QUERY_REQ</returns>
    public Result<string> Serialize(QueryReqMessage message)
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
            
            return json;
        });
}
