
using Janus.Commons.QueryModels;
using Janus.Commons.QueryModels.JsonConversion;
using Janus.Commons.SchemaModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a QUERY_REQ message
/// </summary>
public class QueryReqMessage : BaseMessage
{
    private readonly Query _query;

    /// <summary>
    /// Requested query to be run
    /// </summary>
    public Query Query => _query;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    [JsonConstructor]
    public QueryReqMessage(string exchangeId, string nodeId, Query query!!) : base(exchangeId, nodeId, Preambles.QUERY_REQUEST)
    {
        _query = query;
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public QueryReqMessage(string nodeId, Query query!!) : base(nodeId, Preambles.QUERY_REQUEST)
    {
        _query = query;
    }
}
public static partial class MessageExtensions
{
#pragma warning disable
    public static Result<QueryReqMessage> ToQueryReqMessage(this byte[] bytes)
        => ResultExtensions.AsResult(
            () =>
            {
                var indexOfNullTerm = bytes.ToList().IndexOf(0x00);
                    // sometimes the message is exactly as long as the byte array and there is no null term
                    var bytesMessageLength = indexOfNullTerm > 0 ? indexOfNullTerm : bytes.Length;
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytesMessageLength);
                var message = JsonSerializer.Deserialize<QueryReqMessage>(messageString);
                return message;
            });
#pragma warning enable


}

