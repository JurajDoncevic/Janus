using System.Net;

namespace Janus.Communication.Remotes;

public abstract class RemotePoint
{
    private readonly string _nodeId;
    private readonly string _address;
    private readonly int _listenPort;

    public string NodeId => _nodeId;
    public string Address => _address;
    public int Port => _listenPort;

    internal RemotePoint(string address, int listenPort)
    {
        _nodeId = string.Empty;
        _address = address;
        _listenPort = listenPort;
    }

    internal RemotePoint(string nodeId, string address, int listenPort)
    {
        _nodeId = nodeId;
        _address = address;
        _listenPort = listenPort;
    }

    public override bool Equals(object? obj)
    {
        return obj is RemotePoint point &&
               _nodeId == point._nodeId &&
               EqualityComparer<string>.Default.Equals(_address, point._address) &&
               _listenPort == point._listenPort &&
               NodeId == point.NodeId &&
               EqualityComparer<string>.Default.Equals(Address, point.Address) &&
               Port == point.Port;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_nodeId, _address, _listenPort, NodeId, Address, Port);
    }

    public override string ToString()
    {
        return $"(NodeId:{NodeId};Address:{Address};Port:{Port})";
    }
}
