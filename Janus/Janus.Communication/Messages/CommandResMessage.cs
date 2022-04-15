
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

public class CommandResMessage : BaseMessage
{
    private readonly string _nodeId;

    /// <summary>
    /// Sender node's ID
    /// </summary>
    public string NodeId => _nodeId;

    [JsonConstructor]
    public CommandResMessage(string exchangeId, string nodeId) : base(exchangeId, Preambles.COMMAND_RESPONSE)
    {
        _nodeId = nodeId;
    }

    public CommandResMessage(string nodeId) : base(Preambles.COMMAND_RESPONSE)
    {
        _nodeId = nodeId;
    }
}
