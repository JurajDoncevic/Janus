using Microsoft.Extensions.Configuration;

namespace Janus.Communication.Nodes;

/// <summary>
/// Options to build a communication node
/// </summary>
public class CommunicationNodeOptions
{
    private readonly string _nodeId;
    private readonly int _port;
    private readonly int _timeoutMs;

    /// <summary>
    /// Node ID. Seen as remote ID on other nodes
    /// </summary>
    public string Id => _nodeId;
    /// <summary>
    /// Port to listen on
    /// </summary>
    public int Port => _port;
    /// <summary>
    /// Time in milliseconds after which ingoing and outgoing messages timeout
    /// </summary>
    public int TimeoutMs => _timeoutMs;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Node ID. Seen as remote ID on other nodes</param>
    /// <param name="port">Port to listen on</param>
    /// <param name="timeoutMs">Time in milliseconds after which ingoing and outgoing messages timeout</param>
    public CommunicationNodeOptions(string nodeId, int port = 2500, int timeoutMs = 1000)
    {
        _nodeId = String.IsNullOrEmpty(nodeId.Trim()) ? Guid.NewGuid().ToString() : nodeId;
        _port = port > 0 ? port : 2500;
        _timeoutMs = timeoutMs > 0 ? timeoutMs : 1000;
    }
}

public static class CommunicationNodeOptionsExtensions
{
    public static CommunicationNodeOptions ToCommunicationNodeOptions(this IConfigurationSection configurationSection)
    {
        var nodeId = configurationSection["NodeId"];
        int port = int.TryParse(configurationSection["Port"], out port) ? port : 2500;
        int timeoutMs = int.TryParse(configurationSection["TimeoutMs"], out timeoutMs) ? timeoutMs : 1000;

        return new CommunicationNodeOptions(nodeId, port, timeoutMs);
    }
}
