
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a COMMAND_RES message
/// </summary>
public class CommandResMessage : BaseMessage
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
    [JsonConstructor]
    public CommandResMessage(string exchangeId, string nodeId, bool isSuccess, string outcomeDescription!! = "") : base(exchangeId, nodeId, Preambles.COMMAND_RESPONSE)
    {
        _isSuccess = isSuccess;
        _outcomeDescription = outcomeDescription;
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="isSuccess">Success of the requested command</param>
    /// <param name="outcomeDescription">Requested command's outcome descriptio (e.g. error message)</param>
    public CommandResMessage(string nodeId, bool isSuccess, string outcomeDescription!! = "") : base(nodeId, Preambles.COMMAND_RESPONSE)
    {
        _isSuccess = isSuccess;
        _outcomeDescription = outcomeDescription;
    }
}

public static partial class MessageExtensions
{
#pragma warning disable
    public static DataResult<CommandResMessage> ToCommandResMessage(this byte[] bytes)
        => ResultExtensions.AsDataResult(
            () =>
            {
                var indexOfNullTerm = bytes.ToList().IndexOf(0x00);
                // sometimes the message is exactly as long as the byte array and there is no null term
                var bytesMessageLength = indexOfNullTerm > 0 ? indexOfNullTerm : bytes.Length;
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytesMessageLength);
                var message = JsonSerializer.Deserialize<CommandResMessage>(messageString);
                return message;
            });
#pragma warning enable


}
