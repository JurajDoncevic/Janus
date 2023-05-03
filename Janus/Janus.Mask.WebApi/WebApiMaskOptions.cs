using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Mask.WebApi.InstanceManagement;

namespace Janus.Mask.WebApi;
public sealed class WebApiMaskOptions : MaskOptions
{
    private readonly WebApiOptions _webApiOptions;
    public WebApiMaskOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats dataFormat,
        NetworkAdapterTypes networkAdapterType,
        bool eagerStartup,
        IEnumerable<UndeterminedRemotePoint> startupRemotePoints,
        string startupNodeSchemaLoad,
        string persistenceConnectionString,
        WebApiOptions webApiOptions) 
        : base(
            nodeId,
            listenPort,
            timeoutMs,
            dataFormat,
            networkAdapterType,
            eagerStartup,
            startupRemotePoints,
            startupNodeSchemaLoad,
            persistenceConnectionString)
    {
        _webApiOptions = webApiOptions;
    }

    public WebApiOptions WebApiOptions => _webApiOptions;
}
