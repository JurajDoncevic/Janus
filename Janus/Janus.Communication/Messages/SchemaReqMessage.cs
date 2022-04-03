
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
