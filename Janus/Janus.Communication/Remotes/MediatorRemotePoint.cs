namespace Janus.Communication.Remotes;

public class MediatorRemotePoint : RemotePoint
{
    public MediatorRemotePoint(string address, int listenPort) : base(address, listenPort)
    {
    }

    public MediatorRemotePoint(string nodeId, string address, int listenPort) : base(nodeId, address, listenPort)
    {
    }
}
