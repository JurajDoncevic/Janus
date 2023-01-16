using Janus.Commons;
using Janus.Communication.Remotes;

namespace Janus.Mask.WebApi;
public class WebApiMaskOptions : MaskOptions
{
    private readonly int _webApiPort;
    public WebApiMaskOptions(string nodeId, int listenPort, int timeoutMs, CommunicationFormats dataFormat, NetworkAdapterTypes networkAdapterType, IEnumerable<UndeterminedRemotePoint> startupRemotePoints, string persistenceConnectionString, int webApiPort) : base(nodeId, listenPort, timeoutMs, dataFormat, networkAdapterType, startupRemotePoints, persistenceConnectionString)
    {
        _webApiPort = webApiPort;
    }

    public int WebApiPort => _webApiPort;
}
