namespace Janus.Commons.Messages;

/// <summary>
/// Message used to terminate a logical connection between node. Has no response.
/// </summary>
public class ByeReqMessage : BaseMessage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender node's ID</param>
    public ByeReqMessage(string exchangeId, string nodeId) : base(exchangeId, nodeId, Preambles.BYE_REQUEST)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender node's ID</param>
    public ByeReqMessage(string nodeId) : base(nodeId, Preambles.BYE_REQUEST)
    {
    }

    public override bool Equals(object? obj)
    {
        return obj is ByeReqMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
               NodeId == message.NodeId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, NodeId);
    }
}
