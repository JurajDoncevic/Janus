﻿using Janus.Commons.Nodes;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal class HelloResMessageDto : BaseMessageDto
{
    public int ListenPort { get; set; }
    public NodeTypes NodeType { get; set; }
    public bool RememberMe { get; set; }
}