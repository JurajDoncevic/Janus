
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

public class QueryResMessage : BaseMessage
{
    private readonly string _nodeId;

    /// <summary>
    /// Sender node's ID
    /// </summary>
    public string NodeId => _nodeId;

    [JsonConstructor]
    public QueryResMessage(string exchangeId, string nodeId) : base(exchangeId, Preambles.QUERY_RESPONSE)
    {
        _nodeId = nodeId;
    }

    public QueryResMessage(string nodeId) : base(Preambles.QUERY_REQUEST)
    {
        this._nodeId = nodeId;
    }
}
