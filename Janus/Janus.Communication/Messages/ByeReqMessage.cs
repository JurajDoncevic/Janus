using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Janus.Communication.Messages
{
    /// <summary>
    /// Message used to terminate a logical connection between node. Has no response.
    /// </summary>
    public class ByeReqMessage : BaseMessage
    {
        private readonly string _nodeId;

        /// <summary>
        /// Sender node's ID
        /// </summary>
        public string NodeId => _nodeId;


        public ByeReqMessage(string nodeId) : base(Preambles.BYE_REQUEST)
        {
            _nodeId = nodeId;
        }
    }

    public static partial class MessageExtensions
    {
#pragma warning disable
        public static DataResult<ByeReqMessage> ToByeReqMessage(this byte[] bytes)
            => ResultExtensions.AsDataResult(
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
}
