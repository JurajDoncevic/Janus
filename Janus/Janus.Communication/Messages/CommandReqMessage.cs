﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Messages
{
    public class CommandReqMessage : BaseMessage
    {
        private readonly string _nodeId;

        /// <summary>
        /// Sender node's ID
        /// </summary>
        public string NodeId => _nodeId;

        public CommandReqMessage(string exchangeId, string nodeId) : base(exchangeId, Preambles.COMMAND_REQUEST)
        {
            _nodeId = nodeId;
        }

        public CommandReqMessage(string nodeId) : base(Preambles.COMMAND_REQUEST)
        {
            _nodeId = nodeId;
        }
    }
}
