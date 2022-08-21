
using Janus.Commons.QueryModels;

namespace Janus.Commons.Messages;

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
    public QueryReqMessage(string exchangeId, string nodeId, Query query) : base(exchangeId, nodeId, Preambles.QUERY_REQUEST)
    {
        _query = query ?? throw new ArgumentNullException(nameof(query));
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public QueryReqMessage(string nodeId, Query query) : base(nodeId, Preambles.QUERY_REQUEST)
    {
        _query = query ?? throw new ArgumentNullException(nameof(query));
    }

    public override bool Equals(object? obj)
    {
        return obj is QueryReqMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
               NodeId == message.NodeId &&
               _query.Equals(message._query);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, NodeId, _query);
    }
}

