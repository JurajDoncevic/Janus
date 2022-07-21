namespace Janus.Commons.Messages;
/// <summary>
/// Describes a SCHEMA_REQ message
/// </summary>
public class SchemaReqMessage : BaseMessage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public SchemaReqMessage(string nodeId) : base(nodeId, Preambles.SCHEMA_REQUEST)
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">Exchange ID</param>
    /// <param name="nodeId">Sender's node ID</param>
    public SchemaReqMessage(string exchangeId, string nodeId) : base(exchangeId, nodeId, Preambles.SCHEMA_REQUEST)
    {
    }

    public override bool Equals(object? obj)
    {
        return obj is SchemaReqMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
               NodeId == message.NodeId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, NodeId);
    }
}