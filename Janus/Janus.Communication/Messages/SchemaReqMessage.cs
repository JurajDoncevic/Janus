
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;
/// <summary>
/// Describes a SCHEMA_REQ message
/// </summary>
public class SchemaReqMessage : BaseMessage
{
    private readonly string _nodeId;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public SchemaReqMessage(string nodeId) : base(Preambles.SCHEMA_REQUEST)
    {
        _nodeId = nodeId;
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">Exchange ID</param>
    /// <param name="nodeId">Sender's node ID</param>
    [JsonConstructor]
    public SchemaReqMessage(string exchangeId, string nodeId) : base(exchangeId, Preambles.SCHEMA_REQUEST)
    {
        _nodeId = nodeId;
    }
    /// <summary>
    /// Sender's node ID
    /// </summary>
    public string NodeId => _nodeId;
}

public static partial class MessageExtensions
{
#pragma warning disable
    public static DataResult<SchemaReqMessage> ToSchemaReqMessage(this byte[] bytes)
        => ResultExtensions.AsDataResult(
            () =>
            {
                var indexOfNullTerm = bytes.ToList().IndexOf(0x00);
                // sometimes the message is exactly as long as the byte array and there is no null term
                var bytesMessageLength = indexOfNullTerm > 0 ? indexOfNullTerm : bytes.Length;
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytesMessageLength);
                var message = JsonSerializer.Deserialize<SchemaReqMessage>(messageString);
                return message;
            });
#pragma warning enable


}