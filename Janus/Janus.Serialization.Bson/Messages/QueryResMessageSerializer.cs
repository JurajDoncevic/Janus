using FunctionalExtensions.Base.Resulting;
using Janus.Commons.Messages;
using Janus.Commons.SchemaModels;
using Janus.Serialization.Bson.DataModels;
using Janus.Serialization.Bson.Messages.DTOs;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Serialization.Bson.Messages;

/// <summary>
/// BSON format QUERY_RES message serializer
/// </summary>
public sealed class QueryResMessageSerializer : IMessageSerializer<QueryResMessage, byte[]>
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
                                       ExchangeId = message.ExchangeId,
                                       NodeId = message.NodeId,
                                       TabularData = tabularDataDto,
                                       BlockNumber = message.BlockNumber,
                                       OutcomeDescription = message.OutcomeDescription,
                                       TotalBlocks = message.TotalBlocks
                                   }),
                   () => Results.AsResult(() => new QueryResMessageDto
                   {
                       ExchangeId = message.ExchangeId,
                       NodeId = message.NodeId,
                       TabularData = null,
                       BlockNumber = message.BlockNumber,
                       OutcomeDescription = message.OutcomeDescription,
                       TotalBlocks = message.TotalBlocks
                   })
                   ).Bind(dto => Results.AsResult(() =>
                   {
                       var json = JsonSerializer.Serialize(dto, _serializerOptions);
                       var messageBytes = Encoding.UTF8.GetBytes(json);

                       return messageBytes;
                   }));
               return serialization;
           });
}
