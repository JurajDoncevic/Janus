using Janus.Commons.Nodes;

namespace Janus.Serialization.Avro.Messages.DTOs;
internal sealed class HelloResMessageDto
{
    public string Preamble { get; set; }
    public string ExchangeId { get; set; }
    public string NodeId { get; set; }
    public int ListenPort { get; set; }
    public NodeTypes NodeType { get; set; }
    public bool RememberMe { get; set; }
    public string ContextMessage { get; set; }
}