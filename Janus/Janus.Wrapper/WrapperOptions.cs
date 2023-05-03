using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Components;

namespace Janus.Wrapper;
/// <summary>
/// Wrapper component options
/// </summary>
public class WrapperOptions : IComponentOptions
{
    private readonly string _nodeId;
    private readonly int _listenPort;
    private readonly int _timeoutMs;
    private readonly CommunicationFormats _communicationFormat;
    private readonly NetworkAdapterTypes _networkAdapterType;
    private readonly bool _eagerStartup;
    private readonly IEnumerable<UndeterminedRemotePoint> _startupRemotePoints;
    private readonly bool _startupInferSchema;
    private readonly string _sourceConnectionString;
    private readonly bool _allowsCommands;
    private readonly string _persistenceConnectionString;
    private readonly string _dataSourceName;

    public WrapperOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats communicationFormat,
        NetworkAdapterTypes networkAdapterType,
        bool eagerStartup,
        IEnumerable<UndeterminedRemotePoint> startupRemotePoints,
        bool startupInferSchema,
        string sourceConnectionString,
        bool allowsCommands,
        string persistenceConnectionString,
        string dataSourceName)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _timeoutMs = timeoutMs;
        _communicationFormat = communicationFormat;
        _networkAdapterType = networkAdapterType;
        _eagerStartup = eagerStartup;
        _startupRemotePoints = startupRemotePoints;
        _startupInferSchema = startupInferSchema;
        _sourceConnectionString = sourceConnectionString;
        _allowsCommands = allowsCommands;
        _persistenceConnectionString = persistenceConnectionString;
        _dataSourceName = dataSourceName;
    }


    /// <summary>
    /// Connection string to the wrapper's data source
    /// </summary>
    public string SourceConnectionString => _sourceConnectionString;

    /// <summary>
    /// Does the component instance allow executing commands on its data source?
    /// </summary>
    public bool AllowsCommands => _allowsCommands;

    public string NodeId => _nodeId;

    public int ListenPort => _listenPort;

    public int TimeoutMs => _timeoutMs;

    public CommunicationFormats CommunicationFormat => _communicationFormat;

    public NetworkAdapterTypes NetworkAdapterType => _networkAdapterType;

    public IReadOnlyList<UndeterminedRemotePoint> StartupRemotePoints => _startupRemotePoints.ToList();

    public string PersistenceConnectionString => _persistenceConnectionString;

    public string DataSourceName => _dataSourceName;

    public bool EagerStartup => _eagerStartup;
    /// <summary>
    /// Enables schema inferrence on eager startup
    /// </summary>
    public bool StartupInferSchema => _startupInferSchema;
}
