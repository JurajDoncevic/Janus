using Janus.Commons.Nodes;

namespace Janus.Commons.Messages;

/// <summary>
/// Describes a HELLO_REQ message
/// </summary>
public sealed class HelloReqMessage : BaseMessage
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
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender node's ID</param>
    /// <param name="listenPort">Sender node's listenning port</param>
    /// <param name="nodeType">Sender node's type</param>
    /// <param name="rememberMe">Should the node be remembered (registered)</param>
    public HelloReqMessage(string exchangeId, string nodeId, int listenPort, NodeTypes nodeType, bool rememberMe) : base(exchangeId, nodeId, Preambles.HELLO_REQUEST)
    {
        _listenPort = listenPort;
        _nodeType = nodeType;
        _rememberMe = rememberMe;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender node's ID</param>
    /// <param name="listenPort">Sender node's listenning port</param>
    /// <param name="nodeType">Sender node's type</param>
    /// <param name="rememberMe">Should the node be remembered (registered)</param>
    public HelloReqMessage(string nodeId, int listenPort, NodeTypes nodeType, bool rememberMe) : base(nodeId, Preambles.HELLO_REQUEST)
    {
        _listenPort = listenPort;
        _nodeType = nodeType;
        _rememberMe = rememberMe;
    }

    public override bool Equals(object? obj)
    {
        return obj is HelloReqMessage message &&
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
