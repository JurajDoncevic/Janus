using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Components;
using System.Text.Json.Serialization;

namespace Janus.Mediator.Core;
public sealed class MediatorOptions : IComponentOptions
{
    private readonly string _nodeId;
    private readonly int _listenPort;
    private readonly int _timeoutMs;
    private readonly CommunicationFormats _dataFormat;
    private readonly NetworkAdapterTypes _networkAdapterType;
    private readonly IEnumerable<RemotePoint> _startupRemotePoints;

    public string NodeId => _nodeId;

    public int ListenPort => _listenPort;

    public int TimeoutMs => _timeoutMs;

    public CommunicationFormats CommunicationFormat => _dataFormat;

    public NetworkAdapterTypes NetworkAdapterType => _networkAdapterType;

    public IReadOnlyList<RemotePoint> StartupRemotePoints => _startupRemotePoints.ToList();


    public MediatorOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats dataFormat,
        NetworkAdapterTypes networkAdapterType,
        IEnumerable<RemotePoint> startupRemotePoints)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _timeoutMs = timeoutMs;
        _dataFormat = dataFormat;
        _networkAdapterType = networkAdapterType;
        _startupRemotePoints = startupRemotePoints;
    }

}
