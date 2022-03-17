using Microsoft.Extensions.Configuration;

namespace Janus.Commons.Communication.Node;

/// <summary>
/// Options to build a communication node
/// </summary>
public class CommunicationNodeOptions
{
    private readonly string _nodeId;
    private readonly uint _port;
    private readonly uint _timeoutMs;

    /// <summary>
    /// Node ID. Seen as remote ID on other nodes
    /// </summary>
    public string Id => _nodeId;
    /// <summary>
    /// Port to listen on
    /// </summary>
    public uint Port => _port;
    /// <summary>
    /// Time in milliseconds after which ingoing and outgoing messages timeout
    /// </summary>
    public uint TimeoutMs => _timeoutMs;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Node ID. Seen as remote ID on other nodes</param>
    /// <param name="port">Port to listen on</param>
    /// <param name="timeoutMs">Time in milliseconds after which ingoing and outgoing messages timeout</param>
    public CommunicationNodeOptions(string nodeId, uint port = 2500, uint timeoutMs = 1000)
    {
        _nodeId = String.IsNullOrEmpty(nodeId.Trim()) ? Guid.NewGuid().ToString() : nodeId;
        _port = port;
        _timeoutMs = timeoutMs;
    }
}

public static class CommunicationNodeOptionsExtensions
{
    public static CommunicationNodeOptions FromConfigurationSection(this IConfigurationSection configurationSection)
    {
        var nodeId = configurationSection["NodeId"];
        uint port = uint.TryParse(configurationSection["Port"], out port) ? port : 2500;
        uint timeoutMs = uint.TryParse(configurationSection["TimeoutMs"], out timeoutMs) ? timeoutMs : 1000;

        return new CommunicationNodeOptions(nodeId, port, timeoutMs);
    }
}
