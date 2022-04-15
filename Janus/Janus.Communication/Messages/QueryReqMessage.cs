using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Communication.Messages
{
    public class QueryReqMessage : BaseMessage
    {
        private readonly string _nodeId;

        /// <summary>
        /// Sender node's ID
        /// </summary>
        public string NodeId => _nodeId;

        [JsonConstructor]
        public QueryReqMessage(string exchangeId, string nodeId) : base(exchangeId, Preambles.QUERY_REQUEST)
        {
            _nodeId = nodeId;
        }

        public QueryReqMessage(string nodeId) : base(Preambles.QUERY_REQUEST)
        {
            this._nodeId = nodeId;
        }
    }
}
