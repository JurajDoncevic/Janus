using Janus.Communication.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

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
    [JsonConstructor]
    public HelloResMessage(string exchangeId,  string nodeId, int listenPort, NodeTypes nodeType, bool rememberMe) : base(exchangeId, nodeId, Preambles.HELLO_RESPONSE)
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

}

public static partial class MessageExtensions
{
    #pragma warning disable
    public static Result<HelloResMessage> ToHelloResMessage(this byte[] bytes)
        => ResultExtensions.AsResult(
            () =>
            {
                var indexOfNullTerm = bytes.ToList().IndexOf(0x00);
                // sometimes the message is exactly as long as the byte array and there is no null term
                var bytesMessageLength = indexOfNullTerm > 0 ? indexOfNullTerm : bytes.Length; 
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytesMessageLength);
                var message = JsonSerializer.Deserialize<HelloResMessage>(messageString);
                return message;
            });
    #pragma warning enable


}
