using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Mask.WebApi.InstanceManagement;

namespace Janus.Mask.WebApi;
public class WebApiMaskOptions : MaskOptions
{
    private readonly WebApiOptions _webApiOptions;
    public WebApiMaskOptions(string nodeId, int listenPort, int timeoutMs, CommunicationFormats dataFormat, NetworkAdapterTypes networkAdapterType, IEnumerable<UndeterminedRemotePoint> startupRemotePoints, string persistenceConnectionString, WebApiOptions webApiOptions) : base(nodeId, listenPort, timeoutMs, dataFormat, networkAdapterType, startupRemotePoints, persistenceConnectionString)
    {
        _webApiOptions = webApiOptions;
    }

    public WebApiOptions WebApiOptions => _webApiOptions;
}
