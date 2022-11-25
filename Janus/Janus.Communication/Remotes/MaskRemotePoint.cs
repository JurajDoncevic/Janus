namespace Janus.Communication.Remotes;

public sealed class MaskRemotePoint : RemotePoint
{
    public MaskRemotePoint(string address, int listenPort) : base(address, listenPort)
    {
    }

    public MaskRemotePoint(string nodeId, string address, int listenPort) : base(nodeId, address, listenPort)
    {
    }

    public override RemotePointTypes RemotePointType => RemotePointTypes.MASK;
}
