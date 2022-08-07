using Janus.Commons.Messages;
using Janus.Commons.Nodes;

namespace Janus.Serialization.Json.Messages.DTOs;
internal class HelloResMessageDto : BaseMessageDto
{
    public new string Preamble { get => Preambles.HELLO_RESPONSE; }
    public int ListenPort { get; set; }
    public NodeTypes NodeType { get; set; }
    public bool RememberMe { get; set; }
    public string ContextMessage { get; set; }
}
