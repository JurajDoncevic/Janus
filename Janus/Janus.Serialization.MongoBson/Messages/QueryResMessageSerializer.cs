using Janus.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Serialization.MongoBson.DataModels;
using Janus.Serialization.MongoBson.Messages.DTOs;
using System.Text.Json;

namespace Janus.Serialization.MongoBson.Messages;

/// <summary>
/// MongoBson format QUERY_RES message serializer
/// </summary>
public sealed class QueryResMessageSerializer : IMessageSerializer<QueryResMessage, byte[]>
{
    private readonly TabularDataSerializer _tabularDataSerializer = new TabularDataSerializer();

    /// <summary>
    /// Deserializes a QUERY_RES message
    /// </summary>
    /// <param name="serialized">Serialized QUERY_RES</param>
    /// <returns>Deserialized QUERY_RES</returns>
    public Result<QueryResMessage> Deserialize(byte[] serialized)
        => Results.AsResult(() => Utils.FromBson<QueryResMessageDto>(serialized))
            .Bind(queryResMessageDto =>  // this complication is done so a failing serialization message gets propagated
                    queryResMessageDto.TabularData is not null
                    ? _tabularDataSerializer.FromDto(queryResMessageDto.TabularData)
                                            .Map(tabularData =>
                                                new QueryResMessage(
                                                    queryResMessageDto.ExchangeId,
                                                    queryResMessageDto.NodeId,
                                                    tabularData,
                                                    queryResMessageDto.OutcomeDescription,
                                                    queryResMessageDto.BlockNumber,
                                                    queryResMessageDto.TotalBlocks))
                    : Results.AsResult(() => new QueryResMessage(
                                                    queryResMessageDto.ExchangeId,
                                                    queryResMessageDto.NodeId,
                                                    null,
                                                    queryResMessageDto.OutcomeDescription,
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
            var serialization = message.TabularData.Match(
                tabularData => _tabularDataSerializer.ToDto(tabularData)
                                .Map(tabularDataDto => new QueryResMessageDto
                                {
                                    Preamble = message.Preamble,
                                    ExchangeId = message.ExchangeId,
                                    NodeId = message.NodeId,
                                    TabularData = tabularDataDto,
                                    BlockNumber = message.BlockNumber,
                                    OutcomeDescription = message.OutcomeDescription,
                                    TotalBlocks = message.TotalBlocks
                                }),
                () => Results.AsResult(() => new QueryResMessageDto
                {
                    Preamble = message.Preamble,
                    ExchangeId = message.ExchangeId,
                    NodeId = message.NodeId,
                    TabularData = null,
                    BlockNumber = message.BlockNumber,
                    OutcomeDescription = message.OutcomeDescription,
                    TotalBlocks = message.TotalBlocks
                })
                ).Bind(dto => Results.AsResult(() => Utils.ToBson(dto)));

            return serialization;
        });
}
