
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a COMMAND_REQ message
/// </summary>
public class CommandReqMessage : BaseMessage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    [JsonConstructor]
    public CommandReqMessage(string exchangeId, string nodeId) : base(exchangeId, nodeId, Preambles.COMMAND_REQUEST)
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public CommandReqMessage(string nodeId) : base(nodeId, Preambles.COMMAND_REQUEST)
    {
    }
}
