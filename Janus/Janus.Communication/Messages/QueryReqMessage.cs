
using Janus.Commons.QueryModels;
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
