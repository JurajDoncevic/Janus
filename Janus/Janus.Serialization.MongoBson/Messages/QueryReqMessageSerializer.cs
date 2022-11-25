using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.Messages.DTOs;
using Janus.Serialization.MongoBson.QueryModels;
using Janus.Serialization.MongoBson.QueryModels.DTOs;

namespace Janus.Serialization.MongoBson.Messages;

/// <summary>
/// MongoBson format QUERY_REQ message serializer
/// </summary>
public sealed class QueryReqMessageSerializer : IMessageSerializer<QueryReqMessage, byte[]>
{
    private readonly QuerySerializer _querySerializer = new QuerySerializer();

    /// <summary>
    /// Deserializes a QUERY_REQ message
    /// </summary>
    /// <param name="serialized">Serialized QUERY_REQ</param>
    /// <returns>Deserialized QUERY_REQ</returns>
    public Result<QueryReqMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromBson<QueryReqMessageDto>(serialized))
            .Bind(queryReqMessageDto =>
                _querySerializer.FromDto(queryReqMessageDto.Query)
                .Map(query =>
                    new QueryReqMessage(
                        queryReqMessageDto.ExchangeId,
                        queryReqMessageDto.NodeId,
                        query)));

    /// <summary>
    /// Serializes a QUERY_REQ message
    /// </summary>
    /// <param name="message">QUERY_REQ message to serialize</param>
    /// <returns>Serialized QUERY_REQ</returns>
    public Result<byte[]> Serialize(QueryReqMessage message)
        => Results.AsResult(()
            => _querySerializer.ToDto(message.Query)
                .Bind<QueryDto, QueryReqMessageDto>(dto =>
                    new QueryReqMessageDto
                    {
                        Preamble = message.Preamble,
                        ExchangeId = message.ExchangeId,
                        NodeId = message.NodeId,
                        Query = dto
                    })
                .Map(queryReqMessageDto => Utils.ToBson(queryReqMessageDto))
        );
}
