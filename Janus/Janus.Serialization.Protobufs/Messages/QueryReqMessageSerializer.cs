using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Protobufs.Messages.DTOs;
using Janus.Serialization.Protobufs.QueryModels;
using Janus.Serialization.Protobufs.QueryModels.DTOs;

namespace Janus.Serialization.Protobufs.Messages;

/// <summary>
/// Protobufs format QUERY_REQ message serializer
/// </summary>
public class QueryReqMessageSerializer : IMessageSerializer<QueryReqMessage, byte[]>
{
    private readonly QuerySerializer _querySerializer = new QuerySerializer();

    /// <summary>
    /// Deserializes a QUERY_REQ message
    /// </summary>
    /// <param name="serialized">Serialized QUERY_REQ</param>
    /// <returns>Deserialized QUERY_REQ</returns>
    public Result<QueryReqMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() => Utils.FromProtobufs<QueryReqMessageDto>(serialized))
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
        => ResultExtensions.AsResult(() 
            => _querySerializer.ToDto(message.Query)
                .Bind<QueryDto, QueryReqMessageDto>(dto => 
                    new QueryReqMessageDto
                    {
                        Preamble = message.Preamble,
                        ExchangeId = message.ExchangeId,
                        NodeId = message.NodeId,
                        Query = dto
                    })
                .Map(queryReqMessageDto => Utils.ToProtobufs(queryReqMessageDto))
        );
}
