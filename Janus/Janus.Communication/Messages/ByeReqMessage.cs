using System.Text.Json;
using System.Text.Json.Serialization;

namespace Janus.Communication.Messages;

/// <summary>
/// Message used to terminate a logical connection between node. Has no response.
/// </summary>
public class ByeReqMessage : BaseMessage
{
    [JsonConstructor]
    public ByeReqMessage(string exchangeId, string nodeId) : base(exchangeId, nodeId, Preambles.BYE_REQUEST)
    {
    }

    public ByeReqMessage(string nodeId) : base(nodeId, Preambles.BYE_REQUEST)
    {
    }
}

public static partial class MessageExtensions
{
#pragma warning disable
    public static Result<ByeReqMessage> ToByeReqMessage(this byte[] bytes)
        => ResultExtensions.AsResult(
            () =>
            {
                var indexOfNullTerm = bytes.ToList().IndexOf(0x00);
                // sometimes the message is exactly as long as the byte array and there is no null term
                var bytesMessageLength = indexOfNullTerm > 0 ? indexOfNullTerm : bytes.Length;
                var messageString = Encoding.UTF8.GetString(bytes, 0, bytesMessageLength);
                var message = JsonSerializer.Deserialize<ByeReqMessage>(messageString);
                return message;
            });
#pragma warning enable


}
