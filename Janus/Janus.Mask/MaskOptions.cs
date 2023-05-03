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
    private readonly bool _eagerStartup;
    private readonly IEnumerable<UndeterminedRemotePoint> _startupRemotePoints;
    private readonly string _startupNodeSchemaLoad;
    private readonly string _persistenceConnectionString;

    public string NodeId => _nodeId;

    public int ListenPort => _listenPort;

    public int TimeoutMs => _timeoutMs;

    public CommunicationFormats CommunicationFormat => _dataFormat;

    public NetworkAdapterTypes NetworkAdapterType => _networkAdapterType;

    public IReadOnlyList<UndeterminedRemotePoint> StartupRemotePoints => _startupRemotePoints.ToList();

    public string PersistenceConnectionString => _persistenceConnectionString;

    public bool EagerStartup => _eagerStartup;

    /// <summary>
    /// Node id of schema to load on startup
    /// </summary>
    public string StartupNodeSchemaLoad => _startupNodeSchemaLoad;

    public MaskOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats dataFormat,
        NetworkAdapterTypes networkAdapterType,
        bool eagerStartup,
        IEnumerable<UndeterminedRemotePoint> startupRemotePoints,
        string startupNodeSchemaLoad,
        string persistenceConnectionString)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _timeoutMs = timeoutMs;
        _dataFormat = dataFormat;
        _networkAdapterType = networkAdapterType;
        _eagerStartup = eagerStartup;
        _startupRemotePoints = startupRemotePoints;
        _startupNodeSchemaLoad = startupNodeSchemaLoad;
        _persistenceConnectionString = persistenceConnectionString;
    }

}
