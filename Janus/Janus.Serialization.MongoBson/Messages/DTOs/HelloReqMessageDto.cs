using Janus.Commons.Nodes;

namespace Janus.Serialization.MongoBson.Messages.DTOs;
internal sealed class HelloReqMessageDto : BaseMessageDto
{
    public int ListenPort { get; set; }
    public NodeTypes NodeType { get; set; }
    public bool RememberMe { get; set; }
}