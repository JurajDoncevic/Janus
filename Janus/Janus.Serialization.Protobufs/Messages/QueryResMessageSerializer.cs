using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.Protobufs.DataModels;
using Janus.Serialization.Protobufs.Messages.DTOs;

namespace Janus.Serialization.Protobufs.Messages;

/// <summary>
/// Protobufs format QUERY_RES message serializer
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
        => Results.AsResult(() => Utils.FromProtobufs<QueryResMessageDto>(serialized))
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
        => Results.AsResult(() =>
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
            return Utils.ToProtobufs(queryResMessageDto);
        });
}
