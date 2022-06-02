
using Janus.Commons.CommandModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Describes a COMMAND_REQ message
/// </summary>
public class CommandReqMessage : BaseMessage
{
    private readonly BaseCommand _command;
    private readonly string _commandMessageType;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="exchangeId">ID of the message exchange (request and its response)</param>
    /// <param name="nodeId">Sender's node ID</param>
    [JsonConstructor]
    public CommandReqMessage(string exchangeId, string nodeId, BaseCommand command!!) : base(exchangeId, nodeId, Preambles.COMMAND_REQUEST)
    {
        _command = command;
        _commandMessageType
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Sender's node ID</param>
    public CommandReqMessage(string nodeId) : base(nodeId, Preambles.COMMAND_REQUEST)
    {
    }
}

public static partial class MessageExtensions
{
#pragma warning disable
    public static DataResult<CommandReqMessage> ToCommandReqMessage(this byte[] bytes)
        => ResultExtensions.AsDataResult(
            () =>
            {
                var indexOfNullTerm = bytes.ToList().IndexOf(0x00);
                // sometimes the message is exactly as long as the byte array and there is no null term
                var bytesMessageLength = indexOfNullTerm > 0 ? indexOfNullTerm : bytes.Length;
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytesMessageLength);
                var message = JsonSerializer.Deserialize<CommandReqMessage>(messageString);
                return message;
            });
#pragma warning enable


}

