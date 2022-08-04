using Janus.Commons.Nodes;

namespace Janus.Commons.Messages;

/// <summary>
/// Describes a HELLO_RES message
/// </summary>
public class HelloResMessage : BaseMessage
{
    private readonly int _listenPort;
    private readonly NodeTypes _nodeType;
    private readonly bool _rememberMe;

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
    /// Constructor for response
    /// </summary>
    /// <param name="nodeId">Sender node's ID</param>
    /// <param name="listenPort">Sender node's listenning port</param>
    /// <param name="nodeType">Sender node's type</param>
    public HelloResMessage(string exchangeId, string nodeId, int listenPort, NodeTypes nodeType, bool rememberMe) : base(exchangeId, nodeId, Preambles.HELLO_RESPONSE)
    {
        _listenPort = listenPort;
        _nodeType = nodeType;
        _rememberMe = rememberMe;
    }

    /// <summary>
    /// Constructor for request
    /// </summary>
    /// <param name="nodeId">Sender node's ID</param>
    /// <param name="listenPort">Sender node's listenning port</param>
    /// <param name="nodeType">Sender node's type</param>
    public HelloResMessage(string nodeId, int listenPort, NodeTypes nodeType, bool rememberMe) : base(nodeId, Preambles.HELLO_RESPONSE)
    {
        _listenPort = listenPort;
        _nodeType = nodeType;
        _rememberMe = rememberMe;
    }

    public override bool Equals(object? obj)
    {
        return obj is HelloResMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
               _listenPort == message._listenPort &&
               _nodeType == message._nodeType &&
               _rememberMe == message._rememberMe;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, _listenPort, _nodeType, _rememberMe);
    }
}
