
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

public class CommandReqMessage : BaseMessage
{
    private readonly string _nodeId;

    /// <summary>
    /// Sender node's ID
    /// </summary>
    public string NodeId => _nodeId;

    [JsonConstructor]
    public CommandReqMessage(string exchangeId, string nodeId) : base(exchangeId, Preambles.COMMAND_REQUEST)
    {
        _nodeId = nodeId;
    }

    public CommandReqMessage(string nodeId) : base(Preambles.COMMAND_REQUEST)
    {
        _nodeId = nodeId;
    }
}
