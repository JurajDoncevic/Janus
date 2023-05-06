using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Mask.WebApi.InstanceManagement;

namespace Janus.Mask.WebApi;
public sealed class WebApiMaskOptions : MaskOptions
{
    private readonly WebApiOptions _webApiOptions;
    private readonly bool _startupWebApi;

    public WebApiMaskOptions(
        string nodeId,
        int listenPort,
        int timeoutMs,
        CommunicationFormats dataFormat,
        NetworkAdapterTypes networkAdapterType,
        bool eagerStartup,
        IEnumerable<UndeterminedRemotePoint> startupRemotePoints,
        string startupNodeSchemaLoad,
        bool startupWebApi,
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
        _startupWebApi = startupWebApi;
    }

    public WebApiOptions WebApiOptions => _webApiOptions;

    public bool StartupWebApi => _startupWebApi;
}
