using Janus.Communication.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Message used to instantiate a connection between two nodes
/// </summary>
public class HelloResMessage : BaseMessage
{
    private readonly string _nodeId;
    private readonly int _listenPort;
    private readonly NodeTypes _nodeType;

    /// <summary>
    /// Sender node's ID
    /// </summary>
    public string NodeId => _nodeId;
    /// <summary>
    /// Sender node's listening port
    /// </summary>
    public int ListenPort => _listenPort;
    /// <summary>
    /// Sender node's type
    /// </summary>
    public NodeTypes NodeType => _nodeType;

    /// <summary>
    /// Constructor for response
    /// </summary>
    /// <param name="nodeId">Sender node's ID</param>
    /// <param name="listenPort">Sender node's listenning port</param>
    /// <param name="nodeType">Sender node's type</param>
    [JsonConstructor]
    public HelloResMessage(string exchangeId,  string nodeId, int listenPort, NodeTypes nodeType) : base(exchangeId, Preambles.HELLO_RESPONSE)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _nodeType = nodeType;
    }

    /// <summary>
    /// Constructor for request
    /// </summary>
    /// <param name="nodeId">Sender node's ID</param>
    /// <param name="listenPort">Sender node's listenning port</param>
    /// <param name="nodeType">Sender node's type</param>
    public HelloResMessage(string nodeId, int listenPort, NodeTypes nodeType) : base(Guid.NewGuid().ToString(), Preambles.HELLO_RESPONSE)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _nodeType = nodeType;
    }

}

public static partial class MessageExtensions
{
    #pragma warning disable
    public static DataResult<HelloResMessage> ToHelloResMessage(this byte[] bytes)
        => ResultExtensions.AsDataResult(
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
