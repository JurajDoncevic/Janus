using Janus.Commons.Communication.Node;
using System.Text.Json;

namespace Janus.Commons.Communication.Messages;

public class HelloMessage : IMessage
{
    private readonly string _nodeId;
    private readonly int _listenPort;
    private readonly NodeTypes _nodeTypes;

    public string Preamble => "HELLO";
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
    public NodeTypes NodeTypes => _nodeTypes;

    public HelloMessage(string nodeId, int listenPort, NodeTypes nodeTypes)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _nodeTypes = nodeTypes;

    }

    public byte[] ToBson()
        => Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(this).ToCharArray()
            );

}

public static partial class MessageExtensions
{
    public static DataResult<HelloMessage> FromBson(this byte[] bson)
        => ResultExtensions.AsDataResult(
            () => JsonSerializer.Deserialize<HelloMessage>(Encoding.UTF8.GetString(bson))
            );
}
