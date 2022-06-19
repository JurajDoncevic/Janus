namespace Janus.Communication.Remotes;

public class WrapperRemotePoint : RemotePoint
{
    public WrapperRemotePoint(string address, int listenPort) : base(address, listenPort)
    {
    }

    public WrapperRemotePoint(string nodeId, string address, int listenPort) : base(nodeId, address, listenPort)
    {
    }

    public override RemotePointTypes RemotePointType => RemotePointTypes.WRAPPER;
}
