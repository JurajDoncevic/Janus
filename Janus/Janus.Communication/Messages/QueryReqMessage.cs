
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a QUERY_REQ message
/// </summary>
public class QueryReqMessage : BaseMessage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    [JsonConstructor]
    public QueryReqMessage(string exchangeId, string nodeId) : base(exchangeId, nodeId, Preambles.QUERY_REQUEST)
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public QueryReqMessage(string nodeId) : base(nodeId, Preambles.QUERY_REQUEST)
    {
    }
}
