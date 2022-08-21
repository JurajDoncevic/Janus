using Janus.Commons.CommandModels;

namespace Janus.Commons.Messages;

/// <summary>
/// Describes a COMMAND_REQ message
/// </summary>
public class CommandReqMessage : BaseMessage
{
    private readonly BaseCommand _command;
    private readonly CommandReqTypes _commandReqType;

    /// <summary>
    /// The requested command
    /// </summary>
    public BaseCommand Command => _command;

    /// <summary>
    /// Command type
    /// </summary>
    public CommandReqTypes CommandReqType => _commandReqType;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    /// <param name="command">The requested command</param>
    public CommandReqMessage(string exchangeId, string nodeId, BaseCommand command) : base(exchangeId, nodeId, Preambles.COMMAND_REQUEST)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _commandReqType = command.DetermineMessageType();
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    /// <param name="command">The requested command</param>
    public CommandReqMessage(string nodeId, BaseCommand command) : base(nodeId, Preambles.COMMAND_REQUEST)
    {
        _command = command;
        _commandReqType = command.DetermineMessageType();
    }

    public override bool Equals(object? obj)
    {
        return obj is CommandReqMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
               NodeId == message.NodeId &&
               _command.Equals(message._command) &&
               _commandReqType == message._commandReqType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, NodeId, _command, _commandReqType);
    }
}
