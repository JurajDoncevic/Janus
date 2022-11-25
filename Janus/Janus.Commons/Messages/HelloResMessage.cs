using Janus.Commons.Nodes;

namespace Janus.Commons.Messages;

/// <summary>
/// Describes a HELLO_RES message
/// </summary>
public sealed class HelloResMessage : BaseMessage
{
    private readonly int _listenPort;
    private readonly NodeTypes _nodeType;
    private readonly bool _rememberMe;
    private readonly string _contextMessage;

    /// <summary>
    /// Sender node's listening port
    /// </summary>
    public int ListenPort => _listenPort;
    /// <summary>
    /// Sender node's type
    /// </summary>
    public NodeTypes NodeType => _nodeType;
    /// <summary>
    /// Should the sender node be remembered by the receiving node 
    /// </summary>
    public bool RememberMe => _rememberMe;
    /// <summary>
    /// Context message for the response. Can carry reason of register refusal.
    /// </summary>
    public string ContextMessage => _contextMessage;

    /// <summary>
    /// Constructor for response
    /// </summary>
    /// <param name="nodeId">Sender node's ID</param>
    /// <param name="listenPort">Sender node's listenning port</param>
    /// <param name="nodeType">Sender node's type</param>
    public HelloResMessage(string exchangeId, string nodeId, int listenPort, NodeTypes nodeType, bool rememberMe, string contextMessage = "") : base(exchangeId, nodeId, Preambles.HELLO_RESPONSE)
    {
        _listenPort = listenPort;
        _nodeType = nodeType;
        _rememberMe = rememberMe;
        _contextMessage = contextMessage;
    }

    /// <summary>
    /// Constructor for request
    /// </summary>
    /// <param name="nodeId">Sender node's ID</param>
    /// <param name="listenPort">Sender node's listenning port</param>
    /// <param name="nodeType">Sender node's type</param>
    public HelloResMessage(string nodeId, int listenPort, NodeTypes nodeType, bool rememberMe, string contextMessage = "") : base(nodeId, Preambles.HELLO_RESPONSE)
    {
        _listenPort = listenPort;
        _nodeType = nodeType;
        _rememberMe = rememberMe;
        _contextMessage = contextMessage;
    }

    public override bool Equals(object? obj)
    {
        return obj is HelloResMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
               _listenPort == message._listenPort &&
               _nodeType == message._nodeType &&
               _rememberMe == message._rememberMe &&
               _contextMessage.Equals(message._contextMessage);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, _listenPort, _nodeType, _rememberMe, _contextMessage);
    }
}
