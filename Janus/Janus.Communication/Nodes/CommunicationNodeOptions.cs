using Microsoft.Extensions.Configuration;

namespace Janus.Communication.Nodes;

/// <summary>
/// Options to build a communication node
/// </summary>
public class CommunicationNodeOptions
{
    internal const int DEFAULT_LISTEN_PORT = 3112;
    internal const int DEFAULT_TIMEOUT_MS = 5000;

    private readonly string _nodeId;
    private readonly int _listenPort;
    private readonly int _timeoutMs;

    /// <summary>
    /// Node ID. Seen as remote ID on other nodes
    /// </summary>
    public string NodeId => _nodeId;
    /// <summary>
    /// Port to listen on
    /// </summary>
    public int ListenPort => _listenPort;
    /// <summary>
    /// Time in milliseconds after which ingoing and outgoing messages timeout
    /// </summary>
    public int TimeoutMs => _timeoutMs;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Node ID. Seen as remote ID on other nodes</param>
    /// <param name="listenPort">Port to listen on</param>
    /// <param name="timeoutMs">Time in milliseconds after which ingoing and outgoing messages timeout</param>
    public CommunicationNodeOptions(string nodeId, int listenPort = DEFAULT_LISTEN_PORT, int timeoutMs = DEFAULT_TIMEOUT_MS)
    {
        _nodeId = String.IsNullOrEmpty(nodeId.Trim()) ? Guid.NewGuid().ToString() : nodeId;
        _listenPort = listenPort > 0 ? listenPort : DEFAULT_LISTEN_PORT;
        _timeoutMs = timeoutMs > 0 ? timeoutMs : DEFAULT_TIMEOUT_MS;
    }
}

public static class CommunicationNodeOptionsExtensions
{
    public static CommunicationNodeOptions GetCommunicationNodeOptions(this IConfigurationSection configurationSection)
    {
        var nodeId = configurationSection["NodeId"];
        int port = int.TryParse(configurationSection["ListenPort"], out port) ? port : CommunicationNodeOptions.DEFAULT_LISTEN_PORT;
        int timeoutMs = int.TryParse(configurationSection["TimeoutMs"], out timeoutMs) ? timeoutMs : CommunicationNodeOptions.DEFAULT_TIMEOUT_MS;

        return new CommunicationNodeOptions(nodeId, port, timeoutMs);
    }
}
