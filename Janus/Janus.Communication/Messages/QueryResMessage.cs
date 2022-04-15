
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a QUERY_RES message
/// </summary>
public class QueryResMessage : BaseMessage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    [JsonConstructor]
    public QueryResMessage(string exchangeId, string nodeId) : base(exchangeId, nodeId, Preambles.QUERY_RESPONSE)
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public QueryResMessage(string nodeId) : base(nodeId, Preambles.QUERY_REQUEST)
    {
    }
}
