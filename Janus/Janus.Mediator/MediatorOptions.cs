using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Components;

namespace Janus.Mediator;
/// <summary>
/// Mediator component options
/// </summary>
public sealed class MediatorOptions : IComponentOptions
{
    private readonly string _nodeId;
    private readonly int _listenPort;
    private readonly int _timeoutMs;
    private readonly CommunicationFormats _dataFormat;
    private readonly NetworkAdapterTypes _networkAdapterType;
    private readonly bool _eagerStartup;
    private readonly IEnumerable<UndeterminedRemotePoint> _startupRemotePoints;
    private readonly IEnumerable<string> _startupNodesSchemaLoad;
    private readonly string _startupMediationScript;
    private readonly string _persistenceConnectionString;

    public string NodeId => _nodeId;

    public int ListenPort => _listenPort;

    public int TimeoutMs => _timeoutMs;

    public CommunicationFormats CommunicationFormat => _dataFormat;

    public NetworkAdapterTypes NetworkAdapterType => _networkAdapterType;

    public bool EagerStartup => _eagerStartup;

    public IReadOnlyList<UndeterminedRemotePoint> StartupRemotePoints => _startupRemotePoints.ToList();
    /// <summary>
    /// Node ids to load schemas from at startup
    /// </summary>
    public IReadOnlyList<string> StartupNodesSchemaLoad => _startupNodesSchemaLoad.ToList();
    /// <summary>
    /// Startup mediation script
    /// </summary>
    public string StartupMediationScript => _startupMediationScript;

    public string PersistenceConnectionString => _persistenceConnectionString;

    public MediatorOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats dataFormat,
        NetworkAdapterTypes networkAdapterType,
        bool eagerStartup,
        IEnumerable<UndeterminedRemotePoint> startupRemotePoints,
        IEnumerable<string> startupNodesSchemaLoad,
        string startupMediationScript,
        string persistenceConnectionString)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _timeoutMs = timeoutMs;
        _dataFormat = dataFormat;
        _networkAdapterType = networkAdapterType;
        _eagerStartup = eagerStartup;
        _startupRemotePoints = startupRemotePoints;
        _startupNodesSchemaLoad = startupNodesSchemaLoad;
        _startupMediationScript = startupMediationScript;
        _persistenceConnectionString = persistenceConnectionString;
    }
}
