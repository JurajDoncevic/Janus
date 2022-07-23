using FunctionalExtensions.Base.Results;
using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.DataModels;
using Janus.Serialization.MongoBson.Messages.DTOs;

namespace Janus.Serialization.MongoBson.Messages;

/// <summary>
/// MongoBson format QUERY_RES message serializer
/// </summary>
public class QueryResMessageSerializer : IMessageSerializer<QueryResMessage, byte[]>
{
    private readonly TabularDataSerializer _tabularDataSerializer = new TabularDataSerializer();

    /// <summary>
    /// Deserializes a QUERY_RES message
    /// </summary>
    /// <param name="serialized">Serialized QUERY_RES</param>
    /// <returns>Deserialized QUERY_RES</returns>
    public Result<QueryResMessage> Deserialize(byte[] serialized)
        => ResultExtensions.AsResult(() => Utils.FromBson<QueryResMessageDto>(serialized))
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
            return Utils.ToBson(queryResMessageDto);
        });
}
