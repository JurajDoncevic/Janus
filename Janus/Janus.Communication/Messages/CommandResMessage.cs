
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a COMMAND_RES message
/// </summary>
public class CommandResMessage : BaseMessage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    [JsonConstructor]
    public CommandResMessage(string exchangeId, string nodeId) : base(exchangeId, nodeId, Preambles.COMMAND_RESPONSE)
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    public CommandResMessage(string nodeId) : base(nodeId, Preambles.COMMAND_RESPONSE)
    {
    }
}
