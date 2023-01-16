using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Components;

namespace Janus.Mask;
public class MaskOptions : IComponentOptions
{
    private readonly string _nodeId;
    private readonly int _listenPort;
    private readonly int _timeoutMs;
    private readonly CommunicationFormats _dataFormat;
    private readonly NetworkAdapterTypes _networkAdapterType;
    private readonly IEnumerable<UndeterminedRemotePoint> _startupRemotePoints;
    private readonly string _persistenceConnectionString;

    public string NodeId => _nodeId;

    public int ListenPort => _listenPort;

    public int TimeoutMs => _timeoutMs;

    public CommunicationFormats CommunicationFormat => _dataFormat;

    public NetworkAdapterTypes NetworkAdapterType => _networkAdapterType;

    public IReadOnlyList<UndeterminedRemotePoint> StartupRemotePoints => _startupRemotePoints.ToList();

    public string PersistenceConnectionString => _persistenceConnectionString;

    public MaskOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats dataFormat,
        NetworkAdapterTypes networkAdapterType,
        IEnumerable<UndeterminedRemotePoint> startupRemotePoints,
        string persistenceConnectionString)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _timeoutMs = timeoutMs;
        _dataFormat = dataFormat;
        _networkAdapterType = networkAdapterType;
        _startupRemotePoints = startupRemotePoints;
        _persistenceConnectionString = persistenceConnectionString;
    }

}
