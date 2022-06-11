﻿using Janus.Communication.NetworkAdapters;
using Janus.Communication.Nodes.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Communication.Tests.Mocks
{
    public class AlwaysTimeoutCommunicationNode : BaseCommunicationNode
    {
        public AlwaysTimeoutCommunicationNode(CommunicationNodeOptions options!!, INetworkAdapter networkAdapter) : base(options, networkAdapter)
        {

        } 
        public override NodeTypes NodeType => NodeTypes.MEDIATOR_NODE;
    }
}
