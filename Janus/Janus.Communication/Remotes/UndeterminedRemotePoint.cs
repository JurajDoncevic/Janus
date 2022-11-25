namespace Janus.Communication.Remotes;
public sealed class UndeterminedRemotePoint : RemotePoint
{
    public UndeterminedRemotePoint(string address, int listenPort) : base(address, listenPort)
    {
    }


    public override RemotePointTypes RemotePointType => RemotePointTypes.UNDETERMINED;
}
