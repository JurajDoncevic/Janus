using Janus.Commons;
using Janus.Communication.Remotes;

namespace Janus.Mask.Sqlite;
public sealed class SqliteMaskOptions : MaskOptions
{
    private readonly string _materializationConnectionString;
    private readonly bool _startupMaterializeDatabase;

    public SqliteMaskOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats dataFormat,
        NetworkAdapterTypes networkAdapterType,
        bool eagerStartup,
        IEnumerable<UndeterminedRemotePoint> startupRemotePoints,
        string startupNodesSchemaLoad,
        bool startupMaterializeDatabase,
        string persistenceConnectionString,
        string materializationConnectionString)
        : base(nodeId, listenPort, timeoutMs, dataFormat, networkAdapterType, eagerStartup, startupRemotePoints, startupNodesSchemaLoad, persistenceConnectionString)
    {
        _startupMaterializeDatabase = startupMaterializeDatabase;
        _materializationConnectionString = materializationConnectionString;
    }

    public string MaterializationConnectionString => _materializationConnectionString;

    public bool StartupMaterializeDatabase => _startupMaterializeDatabase;
}
