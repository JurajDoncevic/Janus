namespace Janus.Commons.Messages;

/// <summary>
/// Describes a COMMAND_RES message
/// </summary>
public sealed class CommandResMessage : BaseMessage
{
    private readonly bool _isSuccess;
    private readonly string _outcomeDescription;

    /// <summary>
    /// Signals the success of the requested command
    /// </summary>
    public bool IsSuccess => _isSuccess;

    /// <summary>
    /// Signals that the requested command failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Outcome description of the requested command's outcome (e.g. error message)
    /// </summary>
    public string OutcomeDescription => _outcomeDescription;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    /// <param name="isSuccess">Success of the requested command</param>
    /// <param name="outcomeDescription">Requested command's outcome descriptio (e.g. error message)</param>
    public CommandResMessage(string exchangeId, string nodeId, bool isSuccess, string? outcomeDescription = null) : base(exchangeId, nodeId, Preambles.COMMAND_RESPONSE)
    {
        _isSuccess = isSuccess;
        _outcomeDescription = outcomeDescription ?? string.Empty;
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="isSuccess">Success of the requested command</param>
    /// <param name="outcomeDescription">Requested command's outcome descriptio (e.g. error message)</param>
    public CommandResMessage(string nodeId, bool isSuccess, string? outcomeDescription = null) : base(nodeId, Preambles.COMMAND_RESPONSE)
    {
        _isSuccess = isSuccess;
        _outcomeDescription = outcomeDescription ?? string.Empty;
    }

    public override bool Equals(object? obj)
    {
        return obj is CommandResMessage message &&
               _exchangeId == message._exchangeId &&
               _preamble == message._preamble &&
               NodeId == message.NodeId &&
               _isSuccess == message._isSuccess &&
               _outcomeDescription == message._outcomeDescription;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_exchangeId, _preamble, NodeId, _isSuccess, _outcomeDescription);
    }
}
