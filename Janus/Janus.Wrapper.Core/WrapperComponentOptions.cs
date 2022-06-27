using Janus.Communication.Remotes;
using Janus.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core;
public class WrapperOptions : IComponentOptions
{
    private readonly string _nodeId;
    private readonly int _listenPort;
    private readonly int _timeoutMs;
    private List<RemotePoint> _startupRemotePoints;
    private readonly string _dataSourcePath;

    public string NodeId => _nodeId;

    public int ListenPort => _listenPort;

    public int TimeoutMs => _timeoutMs;

    public IReadOnlyList<RemotePoint> StartupRemotePoints => _startupRemotePoints;

    public string DataSourcePath => _dataSourcePath;

    public WrapperOptions(string nodeId, int listenPort, int timeoutMs, string dataSourcePath, List<RemotePoint> startupRemotePoints)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _timeoutMs = timeoutMs;
        _startupRemotePoints = startupRemotePoints;
        _dataSourcePath = dataSourcePath;
    }
}
