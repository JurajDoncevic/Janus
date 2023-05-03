using Janus.Commons;
using Janus.Communication.Remotes;

namespace Janus.Mask.LiteDB;
public sealed class LiteDbMaskOptions : MaskOptions
{
    private readonly string _materializationConnectionString;

    public LiteDbMaskOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats dataFormat,
        NetworkAdapterTypes networkAdapterType,
        bool eagerStartup,
        IEnumerable<UndeterminedRemotePoint> startupRemotePoints,
        string startupNodesSchemaLoad,
        string persistenceConnectionString,
        string materializationConnectionString) 
        : base(nodeId, listenPort, timeoutMs, dataFormat, networkAdapterType, eagerStartup, startupRemotePoints, startupNodesSchemaLoad, persistenceConnectionString)
    {
        _materializationConnectionString = materializationConnectionString;
    }

    public string MaterializationConnectionString => _materializationConnectionString;
}
