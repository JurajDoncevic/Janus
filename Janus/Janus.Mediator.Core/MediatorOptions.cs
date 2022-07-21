using Janus.Commons;
using Janus.Communication.Remotes;
using Janus.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Mediator.Core;
public class MediatorOptions : IComponentOptions
{
    private readonly string _nodeId;
    private readonly int _listenPort;
    private readonly int _timeoutMs;
    private readonly CommunicationFormats _communicationFormat;
    private List<RemotePoint> _startupRemotePoints;
    public string NodeId => _nodeId;

    public int ListenPort => _listenPort;

    public int TimeoutMs => _timeoutMs;

    public CommunicationFormats CommunicationFormat => _communicationFormat;

    public IReadOnlyList<RemotePoint> StartupRemotePoints => _startupRemotePoints;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="nodeId">Component's node id</param>
    /// <param name="listenPort">Component's listen port</param>
    /// <param name="timeoutMs">Component's general timeout in milliseconds</param>
    /// <param name="startupRemotePoints">Remote points to register on startup</param>
    public MediatorOptions(string nodeId, int listenPort, int timeoutMs, CommunicationFormats communicationFormat, List<RemotePoint> startupRemotePoints)
    {
        _nodeId = nodeId;
        _listenPort = listenPort;
        _timeoutMs = timeoutMs;
        _communicationFormat = communicationFormat;
        _startupRemotePoints = startupRemotePoints;
    }
}
