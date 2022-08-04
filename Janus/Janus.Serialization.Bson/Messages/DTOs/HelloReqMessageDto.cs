using Janus.Commons.Messages;
using Janus.Commons.Nodes;

namespace Janus.Serialization.Bson.Messages.DTOs;
internal class HelloReqMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.HELLO_REQUEST; }
    public int ListenPort { get; set; }
    public NodeTypes NodeType { get; set; }
    public bool RememberMe { get; set; }
}
