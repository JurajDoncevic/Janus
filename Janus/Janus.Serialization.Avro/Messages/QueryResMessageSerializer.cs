using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.Avro.DataModels;
using Janus.Serialization.Avro.Messages.DTOs;
using SolTechnology.Avro;

namespace Janus.Serialization.Avro.Messages;

/// <summary>
/// Avro format QUERY_RES message serializer
/// </summary>
public class QueryResMessageSerializer : IMessageSerializer<QueryResMessage, byte[]>
{
    private readonly string _schema = AvroConvert.GenerateSchema(typeof(QueryResMessageDto));
    private readonly TabularDataSerializer _tabularDataSerializer = new TabularDataSerializer();

    /// <summary>
    /// Deserializes a QUERY_RES message
    /// </summary>
    /// <param name="serialized">Serialized QUERY_RES</param>
    /// <returns>Deserialized QUERY_RES</returns>
    public Result<QueryResMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() =>
        {
            var deserializedModel = AvroConvert.DeserializeHeadless<QueryResMessageDto>(serialized, _schema);

            var tabularData = _tabularDataSerializer.FromDto(deserializedModel.TabularData!).Data;

            var queryMessage = new QueryResMessage(
                deserializedModel.ExchangeId,
                deserializedModel.NodeId,
                tabularData,
                deserializedModel.ErrorMessage,
                deserializedModel.BlockNumber,
                deserializedModel.TotalBlocks
                );

            return queryMessage;
        });

    /// <summary>
    /// Serializes a QUERY_RES message
    /// </summary>
    /// <param name="message">QUERY_RES message to serialize</param>
    /// <returns>Serialized QUERY_RES</returns>
    public Result<byte[]> Serialize(QueryResMessage message)
        => ResultExtensions.AsResult(() =>
        {
            var tabularDataDto = _tabularDataSerializer.ToDto(message.TabularData!).Data;
            var queryResMessageDto = new QueryResMessageDto
            {
                Preamble = message.Preamble,
                ExchangeId = message.ExchangeId,
                NodeId = message.NodeId,
                TabularData = tabularDataDto,
                BlockNumber = message.BlockNumber,
                ErrorMessage = message.ErrorMessage,
                TotalBlocks = message.TotalBlocks
            };
            return AvroConvert.SerializeHeadless(queryResMessageDto, _schema);
        });
}
