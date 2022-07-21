using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.Messages.DTOs;
using Janus.Serialization.Avro.QueryModels;
using Janus.Serialization.Avro.QueryModels.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;

/// <summary>
/// Avro format QUERY_REQ message serializer
/// </summary>
public class QueryReqMessageSerializer : IMessageSerializer<QueryReqMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(QueryReqMessageDto));
    private readonly QuerySerializer _querySerializer = new QuerySerializer();

    /// <summary>
    /// Deserializes a QUERY_REQ message
    /// </summary>
    /// <param name="serialized">Serialized QUERY_REQ</param>
    /// <returns>Deserialized QUERY_REQ</returns>
    public Result<QueryReqMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            var deserializedModel = AvroConvert.DeserializeHeadless<QueryReqMessageDto>(serialized, _schema);

            var query = _querySerializer.FromDto(deserializedModel.Query);

            return new QueryReqMessage(
                deserializedModel.ExchangeId,
                deserializedModel.NodeId,
                query.Data!
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
            var dto = new QueryReqMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                Query = _querySerializer.ToDto(message.Query).Data!
            };
            return AvroConvert.SerializeHeadless(dto, _schema);
        });
}
